<%
	'Create an instance of the SharedSession component
	dim sharedSessionManager
	set sharedSessionManager = Server.CreateObject("IOL.SharedSessionServer.COMWrapper.SharedSession")

	if IsNull(sharedSessionManager) then
		Response.Write "No se puede crear el objeto SharedSessionManager"
		Response.End
	end if
	
	'Initialization
	dim sharedSession__cookieValue
	sharedSession__cookieValue = Request.Cookies(sharedSessionManager.SessionCookieName)
	
	'response.write sharedSession__cookieValue
	'response.end
	
	dim sharedSession
	set sharedSession = sharedSessionManager.Start(sharedSession__cookieValue)

	if IsNull(sharedSession) then
		Response.Write "No se puede crear el objeto SharedSession"
		Response.End
	end if
	


%>