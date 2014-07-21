AspNet Identity 2.0 Extensible Project Template
===============================================
This project expands upon the work done by the ASP.NET Identity Team in the Identity Samples project. The goal is to provide a basic project template wherein the core identity Model classes are easily extensible without messing about with the generic type arguments.

While extending the basic ApplicationUser class provided in teh original Identity Samples project was always relatively straightforward, Extending the Role implementation required some significant work. 

In this project, you can simply add additional properties and/or methods to any of the Identity Model classes as defined in the IdentityModels.cs file. 

For more information, see:

* [ASP.NET MVC and Identity 2.0: Understanding the Basics](http://typecastexception.com/post/2014/04/20/ASPNET-MVC-and-Identity-20-Understanding-the-Basics.aspx)

* [ASP.NET Identity 2.0: Customizing Users and Roles](http://typecastexception.com/post/2014/06/22/ASPNET-Identity-20-Customizing-Users-and-Roles.aspx)

The code in this repo is for a project where the Identity models use string keys. However, there is a lot of good information here about how things work under the hood with the generic types:

* [ASP.NET Identity 2.0 Extending Identity Models and Using Integer Keys Instead of Strings](http://typecastexception.com/post/2014/07/13/ASPNET-Identity-20-Extending-Identity-Models-and-Using-Integer-Keys-Instead-of-Strings.aspx)
