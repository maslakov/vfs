# File Server (.NET)

You need to develop the Virtual File Server / Client application.

The program should consist of two parts: the client and the server.

## Server part

The server processes client requests to work with the virtual file system.

System (VFS): creating, deleting, moving and copying files and folders. All clients must work with the same VFS, the server must provide synchronization of work. 
When one of the clients makes changes to the FFS (for example, creates a new file), other clients should receive a notification of changes.

Mandatory requirements:

The server should provide a Windows service, written in any .NET language.

The server must support simultaneous operation at least with 100 clients.

Server settings must be specified in the configuration file.

The server should not work with a real file system (except for configuration file).

The protocol of interaction between the client and the server is at the discretion of the developer.

The program code must be documented.

Desirable requirements:

1. It is advisable to write to C # or VB .NET.

2. Use WCF technology.

3. Using UnitTests (nUnit or unit tests from MS VS2005).

## Client part

The client is a console Windows application that allows you to work with server from the command line.

Commands for working with the server:

**Connect** - connect to the server. After the command is called, the server must return message with the number of connected clients.

Format: ```connect server_name [: port] UserName```

Note: The system can not have two users with the same name.

If the user tries to connect by specifying the name with which someone connected to it, the server should return an error.

Examples:
```
1. connect localhost Dennis

2. connect 127.0.0.1:123 Andrew
```

**Quit** - exits the server

**MD** - create a directory.

Format: ```MD [Drive:] Path```

Note: the command should not create intermediate directories.

Examples:
```
1. MD c: \ test - should create the test folder on drive C.

2. MD temp - should create a folder temp in the current directory.

3. MD c: \ test1 \ test2 \ - should create the folder test2, if the folder test1 is already exist.
```

**CD** - change current directory

Format: ```CD [Drive:] Path```

Note: Each client must have its own current directory, i.e. calling a CD command on one of the clients should not affect the work of others clients.

Examples:
```
1. CD C: - makes a disk With the current directory

2. CD C: \ temp - makes the folder tempo the current directory

3. CD test1 - should make current subdirectory test1 current directory.
```


**RD** - delete directory

Format: ```RD [Drive:] Path```

Note: the command can not delete the current directory and directory, in which has a subdirectory.

Examples:
```
1. RD test1 deletes the subdirectory
```

**DELTREE** - delete the directory and all its subdirectories

Format: ```DELTREE [Drive:] Path```

Examples:
```
1. DELTREE c: \ test
```

**MF** - create file

Format: ```MF [[DRIVE:] Path] FileName```

Examples:
```
1. MF c: \ test \ 1.txt - creates the file 1.txt in the folder c: \ test,

2. MF test \ 2.txt - creates the file 2.txt in the subdirectory test current directory,

3. MF 3.txt - creates a 3.txt file in the current directory.
```

**DEL** - delete the file

Format: ```DEL [[DRIVE:] Path] FileName```

Examples:
```
1. DEL 1.txt - deletes the file 1.txt in the current directory.
```
**LOCK** - prohibits the deletion of a file

Format: `LOCK [[DRIVE:] Path] FileName`

Note: multiple users can lock the same file, and it can not be deleted until all users have removed

Blocking. The user can not lock the file 2 or more times.

The directory can not be deleted if it has locked files. File

Can not be deleted if it is blocked.

Examples:
```
1. LOCK 1.txt - locks the file 1.txt in the current directory.
```

**UNLOCK** - removes the ban on deletion from a file

Format: `UNLOCK [[DRIVE:] Path] FileName`

Note: The user can only remove his or her lock.

Examples:
```
1. UNLOCK 1.txt - unlocks the file 1.txt in the current directory.
```
**COPY** - copies a file or directory to another directory

Format: `COPY [drive:] source [drive:] destination`

Note: The directory should be copied with all content. Destination can not point to a file name.

Examples:
```
1. COPY test1 c: \ temp - copies the subdirectory test1 from the current
```
Directory into the directory c: \ temp

**MOVE** - moves a file or directory to another directory

Format: `MOVE [drive:] source [drive:] destination`

Note: The directory must move with all the content. Destination can not point to a file name. A locked file can not be moved.

Examples:
```
1. MOVE test1 c: \ temp - copies the subdirectory test1 from the current directory into the directory c: \ temp
```

**PRINT** - displays the directory tree

Format: `PRINT`

Examples:
```
1. PRINT - displays the directory tree as shown below.

C:

| _DIR1

| | | | _DIR2

| | | | | |

| | | | _DIR4

| | | | | | | _DIR3

| | | | | | | _1.txt

| | | | | |

| | | | _DIR5

| | | | | | | _ 2.txt [LOCKED by Me]

| | | | | | | _ 3.txt [LOCKED by TestUser]
```

Additional Information:

1. By default, there is a C: drive

2. All commands are register-independent: i.e. Commands CD, cd, Cd, cd - are the same.

3. You can not delete, move locked files, and directories where there are such files.

4. You can not delete the current directory.

5. Any command other than CD must not change the current directory.

6. If there is no prefix C: \ in the path, all actions must be performed in the current directory.

7. In case of any error, the program should display a message on the screen.

8. When outputting, folders and files must be sorted lexicographically.

9. When displaying blocked files, you must specify that it is blocked, and by which user.

10. When changing the VFS, all client  (except for the one who made the change) should receive a notification in the form of a text message about who and what change did, for example, 

in this form: 
```
UserName performing command: RD c: \ test
```

where UserName is the user name specified in the command `connect`


# Solution
- Client
	- Single project with console client. Command interface according to the task definition.

- Server
 - Core
	- Assemblies which provide server side logic:
		- Interfaces for main system components (session manager, file system, storage, service)
		- Custom exceptions
		- WCF-service implementation
		- Virtual file system implementation
		- Implementation of the storage object, which incapsulate the logic of storing of file system elements
		- Session manager implementation
		- Limited test set: tests only for storage component, as a key object
 - Host
	- simple console host for test purpose
	- Windows-service based host - Host.WinService.exe. Can be setup as a service Setup-проект 
	or used as console application for testing purpose. Run with "/m" key for that.
	
	>>Host.WinService.exe /m
	
	All the settings are in config file: host address and a number of restrart trials for WCF-service, in case of host/service failure
	
	
	