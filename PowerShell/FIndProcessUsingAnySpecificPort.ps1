$port_id = 8080
netstat -aon | findstr $port_id
# for example 3192 is the process id we found
tasklist /FI "PID eq 3192"

# kill the process with id
taskkill /PID 3192 /F