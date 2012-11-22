<%
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
	
%>