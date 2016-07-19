AspNet Identity 2.0 with Group-Based Permissions Management
===========================================================
This project expands upon the work done by the ASP.NET Identity Team in the Identity Samples project. The goal is to extend the ASP.NET MVC 5 Identity system and implement a Group-based permission scheme. Users belong to Groups, and Groups have sets of authorization permissions to exxecute code within the application (using [Authorize]). The article can be found at [ASP.NET Identity 2.0: Implementing Group-Based Permissions Management](http://typecastexception.com/post/2014/08/10/ASPNET-Identity-20-Implementing-Group-Based-Permissions-Management.aspx)


In this project, we make extensive addtions to the original Identity Samples project. We start with the easily extensible project template found at [AspNet Identity 2.0 Extensible Project Template](https://github.com/TypecastException/AspNet-Identity-2-Extensible-Project-Template) and add the capability to organize granular Role permissions into Groups, to which Users can then be assigned. 

Update : Added the ability to authorize controller actions based on Roles, check [PermissionFilter] class to see how this works.

There is a lot of room for improvment in this project. Particularly with respect to the design of Views and presentation. Do feel free to add to the issues list and/or hit me with a Pull Request if you have ideas or some code to add. 

For more information on ASP.NET Identity, see:

* [ASP.NET MVC and Identity 2.0: Understanding the Basics](http://typecastexception.com/post/2014/04/20/ASPNET-MVC-and-Identity-20-Understanding-the-Basics.aspx)

* [ASP.NET Identity 2.0: Customizing Users and Roles](http://typecastexception.com/post/2014/06/22/ASPNET-Identity-20-Customizing-Users-and-Roles.aspx)

The code in this repo is for a project where the Identity models use string keys. If you prefer integer keys (and database PK fields) see this repo instead:

[AspNet Identity 2.0 Extensible Project Template (integer keys)](https://github.com/TypecastException/AspNet-Identity-2-With-Integer-Keys)

There is a lot of good information here about how things work under the hood with the generic types, even if you don;t plan to use the integer keys:
* [ASP.NET Identity 2.0 Extending Identity Models and Using Integer Keys Instead of Strings](http://typecastexception.com/post/2014/07/13/ASPNET-Identity-20-Extending-Identity-Models-and-Using-Integer-Keys-Instead-of-Strings.aspx)

Please feel free to open issues, report bugs, and/or shoot me a pull request if you see potential fixes/improvements. 
