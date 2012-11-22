<!--#include file="SessionStart.asp"-->
    <u> ASP App </u>
    <br /><br /><br />

<%
	dim currentValue
	currentValue = sharedSession.Get("val")
	
	if IsNull(currentValue) then
		currentValue = ""
	end if
	
	currentValue = currentValue + " ASPValue - "
	
	Response.Write currentValue

	call sharedSession.Set("val", currentValue)
%>
<!--#include file="SessionEnd.asp"-->
<br />
<br />
<a href="http://localhost/TestASPNET/"> Get More Values </a>