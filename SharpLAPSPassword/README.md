# SharpLAPSPassword

## Description
This tool will search for LAPS password on a specific domain. 

## Usage

```
C:\>SharpLAPSPassword.exe --help
SharpLAPSPassword 1.0.0.0
Author @davidlebr1

  -d, --domaincontroller    Required. Domain controller ip or name.

  -u, --username            Username.

  -p, --password            Password

  --help                    Display this help screen.

  --version                 Display version information.
```

### Example

#### With the current user context
```
C:\>SharpLAPSPassword.exe -d 192.168.1.10
wk01.test.local:uwyL$*z&S%FR&4
srv01.test.local:5rWWxw*Zc4@ZKC
```

#### Passing username and password
```
C:\>SharpLAPSPassword.exe -d 192.168.1.10 -u john -p doe
wk01.test.local:uwyL$*z&S%FR&4
srv01.test.local:5rWWxw*Zc4@ZKC
```