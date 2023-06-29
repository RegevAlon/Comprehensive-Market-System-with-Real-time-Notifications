# Market - Team 12B
Workshop on Software Engineering Project

White diagram- https://drive.google.com/file/d/1RU01D5ycVtTe1qdaflVgqlv8iKysTedF/view?usp=sharing

Architecture- https://drive.google.com/file/d/1wEKhmXrAyNvbjBUJNkWsl6TMJV1DFPbN/view?usp=sharing

Wireframe- https://drive.google.com/file/d/1BFItfUoSUOisjRUYTxFfqrHFHQqq_0Ik/view?usp=sharing

# Running the server
For Running the server you need to open Market Solution and then set ServerMarket as startup project.Click f5 and the server is on!
# Configure the server
For running the server using the ServerMarket project, you must config an inital scenario for the market, including the admin's credentials.
First, go to Server/MarketBackend/MarketBackend/SystemSettings/App.config, and set ShouldRunInitFile = "True" and InitFilePath to path of scenario configuration file.
tweaks:
   - LocalDBMode-choose `true` if you want the DB to be in your local machine.`false` for Azure remote DB.
   - ShouldRunInitFile: If you want the system to perform the actions in the init file, set this to `true`.
   - InitFileName: Choose an init file "example.json".
   - ServerPort: port for your server
   - ExternalServicesActive: If you want to add some extra pizzazz with external services, set this to `true`. The default is `false`.//not in use
structure:
```
{
  "LocalDBMode": true,
  "ShouldRunInitFile": true,
  "InitFileName": "InitFile1.json",
  "WebsocketServerPort": 7888,
  "ExternalServicesActive": false,
  "AdminUsername": "MasterAdmin",
  "AdminPassword": "MasterAdmin",
}
```


# Init File
This file should contain use cases for the system initialization.
The format is json with 2 fields for the admin's credentials, and third field for the use cases.
The loading process takes the values of the use case and activate the function that corresponding to the tag value.
all values are given as strings!
if you want the format of a function you can just look at IMarket interface
```
{
    "UseCases": [
        {
            "sessionid": "2",
            "Username": "u2",
            "Password": "password2",
            "Tag": "Register"
        },
        {
            "sessionid": "3",
            "Username": "u3",
            "Password": "password3",
            "Tag": "Register"
        },
        {
            "sessionid": "4",
            "Username": "u4",
            "Password": "password4",
            "Tag": "Register"
        },
        {
            "sessionid": "5",
            "Username": "u5",
            "Password": "password5",
            "Tag": "Register"
        }
   ]
}
```
# Running the client

1. Run the following command in ClientMarket folder: `npm install ; npm install classnames ; npm install react-rating-stars-component` 

2. After running the server run `npm run dev` command and open the given url.
