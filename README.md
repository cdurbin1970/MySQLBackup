MySQLBackup

A MySQLBackup GUI written in C# using VS2013.

- Libraries -

BouncyCastle - https://www.bouncycastle.org/
MySQL.Data - http://dev.mysql.com/downloads/connector/net/


This is a work in progress and it is not completely finished. 

MySQL Backup allows you to backup a MySQL database using a GUI. It uses mysqldump.exe to actually do the backups.
All information is stored to config files. Passwords are encrypted using the BouncyCastle encryption library. 
