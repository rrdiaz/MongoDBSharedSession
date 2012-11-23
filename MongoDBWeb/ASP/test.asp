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
		
	dim Session
	set Session = sharedSessionManager.Start(sharedSession__cookieValue)

    if IsNull(Session) then
		Response.Write "No se puede crear el objeto SharedSession"
		Response.End
	end if
	
    'Set SharedSession Key from ASP
    'call sharedSession.Set("KeyFromASP","asp123")
    Session("KeyFromASP")="asp123"
    
    'Get SharedSession Key from ASP.Net
    Response.write("<br>Key from .NET<br>")
    'Response.Write(sharedSession.Get("KeyFromASPNet"))
    Response.Write(Session("KeyFromASPNet"))
    
    'Get SharedSession Key from ASP
    Response.write("<br>Key from ASP<br>")
    'Response.Write(sharedSession.Get("KeyFromASP"))
    Response.Write(Session("KeyFromASP"))
    'End Session Start
        
    'Session End
    'Store the session new value
	call sharedSessionManager.Finish(Session.Id)
    	
	'Get the updated session id
	dim sharedSession__newCookieValue
	sharedSession__newCookieValue = Session.GetSessionId()
	if sharedSession__cookieValue <> sharedSession__newCookieValue then
		'Store it in the cookie
		Response.Cookies(Session.SessionCookieName) = sharedSession__newCookieValue
		Response.Cookies(Session.SessionCookieName).Path = "/"
	end if
	
	call Session.Dispose()
	Session = null
    'End Session End
%>