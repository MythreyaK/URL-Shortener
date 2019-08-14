# URL Shortner

A simple URL shortner using .NET Core.  

This project is under heavy development and currently exists only 
to help me learn stuff. Feel free to check it out and give feedback!

## Build 

Set the startup project as `URLShortner.API` and a launch configuration 
of your liking (supports IIS Express, Kestrel and Docker). You'll need the
`ASP.Net and web development` and `.NET Core cross-platform` workloads 
installed in Visual Studio. Docker is required for Docker support. 

## Usage

Curently, an in-memory database is used, but can be easily be swapped out 
to use an actual database. 

To create a redirect, send a `POST` request to the api endpoint (at `/api`)
with data (JSON), where expiresOn is optional. If not set, the link
never expires. 

```
{
  "name": "Example",
  "destinationURL": "http://example.com",
  "expiresOn": "2019-08-15T20:00:00Z"
}
```

To get details on the redirect, send a `GET` request to `/api/<short-url>`.

To update a redirect, create a `PUT` request with the updated details to 
`/api/<short-url>`. Updating the destination, once a redirect is created 
is not supported. 

Redirects can be deleted by sending a `DELETE` request to `/api/<short-url>`. 

To access the destination pointed to by a redirect, navigate to `/<short-url>`. 
The redirect to the destination is a `302 redirect`. 

### TODO
* Authentication for the API endpoint
* Management UI
* Unit tests


## License 
Copyright (C) 2019  Mythreya K

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
