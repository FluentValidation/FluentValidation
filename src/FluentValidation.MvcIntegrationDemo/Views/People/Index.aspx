<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<FluentValidation.MvcIntegrationDemo.Models.Person>" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Index</title>
    <script type="text/javascript" src="/scripts/MicrosoftAjax.js"></script>
    <script type="text/javascript" src="/scripts/MicrosoftMvcAjax.debug.js"></script>
    <script type="text/javascript" src="/scripts/MicrosoftMvcValidation.debug.js"></script>
</head>
<body>
    <div>
		<p>ModelState Valid? <%= ViewData["valid"] %></p>
		
		<%= Html.ValidationSummary() %>
		<% Html.EnableClientValidation(); %>
    
		<% using (Html.BeginForm()) { %>
			Id: <%= Html.TextBoxFor(x => x.Id) %><%= Html.ValidationMessageFor(x => x.Id) %>
			<br />
			Name: <%= Html.TextBoxFor(x => x.Name) %><%= Html.ValidationMessageFor(x => x.Name) %>			
			<br />
			Email: <%=Html.TextBoxFor(x => x.Email) %><%= Html.ValidationMessageFor(x => x.Email) %>
			<br />
			Age: <%= Html.TextBoxFor(x => x.Age) %><%= Html.ValidationMessageFor(x => x.Age) %>
			
			<br /><br />
			
			<input type="submit" value="submit" />
		<% } %>
		
		<%= DateTime.Now %>
		
    </div>
</body>
</html>
