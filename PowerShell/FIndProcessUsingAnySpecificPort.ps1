$port_id = 8080
netstat -aon | findstr $port_id
tasklist /FI "PID eq process_id"