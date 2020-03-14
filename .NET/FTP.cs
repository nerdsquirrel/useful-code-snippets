using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Windows.Forms;

namespace UNICARE_Export_Tool
{
    class FTP
    {

        private string formatErrorMessage(string ErrorMsg)
        {
            return DateTime.Now + " - " + ErrorMsg;
        }

        public bool CheckFTPFileExists(string FTPServerIP, string ftpUserID, string ftpPassword, string FolderToCheck, string FileToCheck, ListBox logBox)
        {
            logBox.Items.Add(formatErrorMessage("Checking for the file - " + "ftp://" + FTPServerIP + FolderToCheck + FileToCheck));

            StringBuilder result = new StringBuilder();
            WebResponse response = null;
            StreamReader reader = null;
            bool bSearchFound = false;


            try
            {
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + FTPServerIP + FolderToCheck));
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                reqFTP.Proxy = null;
                reqFTP.KeepAlive = false;
                reqFTP.UsePassive = false;
                response = reqFTP.GetResponse();
                reader = new StreamReader(response.GetResponseStream());
                string line = reader.ReadLine();

                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");

                    if (FileToCheck.Trim() == line.Trim())
                    {
                        bSearchFound = true;
                    }

                    line = reader.ReadLine();
                }

                reader.Close();

            }
            catch (Exception ex)
            {
                bSearchFound = false;
            }

            if (bSearchFound)
                logBox.Items.Add(formatErrorMessage("FTP File is located successfully"));
            else
                logBox.Items.Add(formatErrorMessage("FTP File is NOT found"));

                return bSearchFound;
            
        }

        public bool DeleteFTPFile(string FTPServerIP, string ftpUserID, string ftpPassword, string FTPFolder, string FileToDelete, ListBox logBox)
        {
            logBox.Items.Add(formatErrorMessage("Attempting to delete the FTP file - " + "ftp://" + FTPServerIP + FTPFolder + FileToDelete));

            //string[] downloadFiles;
            //StringBuilder result = new StringBuilder();
            WebResponse response = null;
            //StreamReader reader = null;
            bool bDeleted = false;


            try
            {
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + FTPServerIP + FTPFolder + FileToDelete));
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;
                reqFTP.Proxy = null;
                reqFTP.KeepAlive = false;
                reqFTP.UsePassive = false;
                response = reqFTP.GetResponse();

                bDeleted = true;
            }
            catch (Exception ex)
            {
                logBox.Items.Add(formatErrorMessage("Failed to the delete the FTP file. Error - " + ex.Message));
                bDeleted = false;
            }

            return bDeleted;
        }

        public bool ConnectFTP(string FTPServerIP, string ftpUserID, string ftpPassword, ListBox logBox)
        {

            logBox.Items.Add(formatErrorMessage("Attempting to connect to FTP Server " + FTPServerIP + " as User " + ftpUserID));
       
            WebResponse response = null;
            StreamReader reader = null;
            try
            {
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + FTPServerIP + "/"));
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                reqFTP.Proxy = null;
                reqFTP.KeepAlive = false;
                reqFTP.UsePassive = false;
                response = reqFTP.GetResponse();
                reader = new StreamReader(response.GetResponseStream());
                reader.Close();

                logBox.Items.Add(formatErrorMessage("FTP Server connection was successful"));

                return true;
            }
            catch (Exception ex)
            {
                logBox.Items.Add(formatErrorMessage("An Error occured while connect to FTP Server - " + ex.Message));
                return false;
            }
        }

        public void DownloadFile(string filename, string User, string Password, string FTPServerIP, string DownloadPath, string RemoteFolder, ListBox logBox)
        {
            try
            {
                logBox.Items.Add(formatErrorMessage("Started Downloading the file " + "ftp://" + FTPServerIP + RemoteFolder + filename + " to " + DownloadPath + filename ));


                // Create a WebClient
                WebClient request = new WebClient();

                // Setup our credentials
                request.Credentials = new NetworkCredential(User, Password);


                // Download the data into a Byte array
                string RemoteCompletePath = "ftp://" + FTPServerIP + RemoteFolder + filename;
                byte[] fileData = request.DownloadData(RemoteCompletePath);

                // Create a Filestream that we'll write the byte array to 
                FileStream file = File.Create(DownloadPath + filename);

                // Write the full byte array to the file
                file.Write(fileData, 0, fileData.Length);

                // Close the file so that other processes can access it.
                file.Close();

                logBox.Items.Add(formatErrorMessage("Completed Downloading File"));

                // Once downloaded, delete from FTP location
                DeleteFTPFile(FTPServerIP, User, Password, RemoteFolder, "Data.csv", logBox);
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    return;
                }
                else
                {
                    return;
                }
            }

        }

        public void UploadFile(string filename, string User, string Password, string FTPServerIP, string UploadPath, string RemoteFolder, ListBox logBox)
        {
            try
            {
                logBox.Items.Add(formatErrorMessage("Started Uploading the file " + UploadPath + filename + " to " + "ftp://" + FTPServerIP + RemoteFolder + filename));

                // Get a new FtpWebRequest object.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + FTPServerIP +  RemoteFolder +  filename);

                // Method will be UploadFile
                request.Method = WebRequestMethods.Ftp.UploadFile;

                // Set our credentials
                request.Credentials = new NetworkCredential(User, Password);

                // Setup a stream for the request and a stream for the file we'll be uploading.
                Stream ftpStream = request.GetRequestStream();
                FileStream file = File.OpenRead(UploadPath + filename);

                // Setup variables we'll use to read the file
                int length = 1024;
                byte[] buffer = new byte[length];
                int bytesRead = 0;

                // Write the file to the request stream
                do
                {
                    bytesRead = file.Read(buffer, 0, length);
                    ftpStream.Write(buffer, 0, bytesRead);
                }
                while (bytesRead != 0);

                // Close the streams.
                file.Close();
                ftpStream.Close();

                logBox.Items.Add(formatErrorMessage("Completed Uploading File"));


                // Delete the export file once uploaded
                System.IO.File.Delete(UploadPath + filename);

            }
            catch (WebException e)
            {
                logBox.Items.Add(formatErrorMessage("Error while upload file - " + e.Message));

                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    return ;
                }
                else
                {
                    return ;
                }
            }
        }
    }


}
