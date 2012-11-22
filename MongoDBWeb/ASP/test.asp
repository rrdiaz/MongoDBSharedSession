<%
	'Session Start
    'Create an instance of the SharedSession component
	dim sharedSessionManager
	set sharedSessionManager = Server.CreateObject("IOL.SharedSessionServer.COMWrapper.MongoSharedSession")

	if IsNull(sharedSessionManager) then
		Response.Write "No se puede crear el objeto SharedSessionManager"
		Response.End
	end if
	
	'Initialization
	dim sharedSession__cookieValue
	sharedSession__cookieValue = Request.Cookies(sharedSessionManager.SessionCookieName)

    response.write("<br>sharedSessionManager.SessionCookieName<br>")
    response.write(sharedSessionManager.SessionCookieName)

	if IsNull(sharedSession__cookieValue) or sharedSession__cookieValue="" then
	    response.write "Vacio"
        response.end
	end if

	response.write("<br>sharedSession__cookieValue<br>")
    response.write(sharedSession__cookieValue)
		
	dim sharedSession
	set sharedSession = sharedSessionManager.Start(sharedSession__cookieValue)

    if IsNull(sharedSession) then
		Response.Write "No se puede crear el objeto SharedSession"
		Response.End
	end if
	
    'Set SharedSession Key from ASP
    call sharedSession.Set("KeyFromASP","asp123")
    'sharedSession["KeyFromASP"]="ValueFromASP"
    
    'Get SharedSession Key from ASP.NET
    Response.write("<br>Key from .NET<br>")
    Response.Write(sharedSession.Get("KeyFromASPNET"))
    
    'Get SharedSession Key from ASP
    Response.write("<br>Key from ASP<br>")
    Response.Write(sharedSession.Get("KeyFromASP"))
    'End Session Start
        
    'Session End
    'Store the session new value
	call sharedSessionManager.Finish(sharedSession.Id)
    	
	'Get the updated session id
	dim sharedSession__newCookieValue
	sharedSession__newCookieValue = sharedSession.GetSessionId()
	if sharedSession__cookieValue <> sharedSession__newCookieValue then
		'Store it in the cookie
		Response.Cookies(sharedSession.SessionCookieName) = sharedSession__newCookieValue
		Response.Cookies(sharedSession.SessionCookieName).Path = "/"
	end if
	
	call sharedSession.Dispose()
	sharedSession = null
    'End Session End
   

%>