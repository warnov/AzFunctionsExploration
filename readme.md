# DllLocalFilesFunction
This sample shows how to use external assemblies in Azure functions, as well as how to read local files with Azure Functions.

##Including external assemblies in Azure Functions
As you can notice there are two project folders inside this solution:

 1. CustomTypes
 2. DllLocalFileFunction
 3. DllLocalFileFunctionClient

The first one is intended to serve as a sample of an external component. It just contains a class with an abstraction of the complex numbers: `ComplexNumber.cs`. We are going to use it  to create a complex number and then use it overrided `ToString()` to write it down to the response of the function.

Compiling this component will produce the `CustomTypes.dll` assembly, and it will be imported in the function problem.

The second project is the one containing the function. An Azure Function project can contain one or more functions. Each one of them will be included in its own folder. We have named the only function inside the project as `DllLocalFileFunction`. Inside this folder you can find specially:

 1. function.json
 2. project.json
 3. run.csx

In this sample it is of special interest the `run.csx` file as it is the one containing the code that will use the functionality defined in the external component `(CustomTypes)` . 
Please note that in order to you can reference the external assembly a folder named `bin` have been created inside the function folder. This folder was created manually. It is not generated when you create the function using the [Visual Studio Tools for Azure Functions](https://blogs.msdn.microsoft.com/webdev/2016/12/01/visual-studio-tools-for-azure-functions/). Then inside this folder, you mustinclude the generated assembly `CustomTypes.dll` and after this has been done, you must declare the reference in the functions code, at the beginning of the file:

    #r "CustomTypes.dll"
And then, add the traditional `using` directive to include the namespace inside the assembly you will be using:

    using CustomTypes;

  With this done, you can simply call:

    ComplexNumber complex = new ComplexNumber(3, 5);
  
*Note that the IntelliSense provided by Visual Studio may not give you information about this class, as the tooling is still in beta.* 

##Reading local files from Azure Functions
This is not a recommended practice, as in most cases if your Function needs to read files it will be better that those files would be stored on an Azure Blob, and downloaded as required.
But if for any reason you need to complement your function with a companion file (maybe a custom configuration XML file), you can read them using the traditional I/O libraries provided by the .NET Framework:

    using System.IO;
    ...
    ...
    var fileText = File.ReadAllText("./DllLocalFileFunction/hello.txt");
 
Note that you just pass the relative path of the file: . for root, `DllLocalFileFunction` as the function folder, and then the file that we have included (in this case, a simple `hello.txt` file containing the text `Hello World!!!`.


There is a special situation though, when you are deploying the function. In the Azure environment, the execution context of the function as of today, is located at `d:\windows\`, while the contents of the file reside in `D:\home\site\wwwroot\` so you are need to change that line of code in order to access the file. A workaround to avoid this manual replacement could be using utilities such as `Server.MapPath` but I haven't made this work. Community help here will be appreciated. In this case the code should be:

    var fileText = File.ReadAllText(@"D:\home\site\wwwroot\DllLocalFileFunction\hello.txt");


##Testing the function
As this function is based on the `Generic WebHook` template, it receives a `name` parameter in the `get` call. Then it returns a greeting for the name, the complex number created, and the content fot the file. For example if you have:

 - Windows 10
 - Visual Studio 2015 + [Visual Studio Tools for Azure Functions](https://blogs.msdn.microsoft.com/webdev/2016/12/01/visual-studio-tools-for-azure-functions/)
 - [Bash for Windows 10](https://msdn.microsoft.com/en-us/commandline/wsl/about)

You could use `CURL` from the bash console to test the function after you have launched it from Visual Studio: *(note that a console application will be launched and act as the HTTP server for you to test from development)*

    root@UNDERBEAST:/mnt/c/Users/warnov# curl http://localhost:7071/api/DllLocalFileFunction?name=warnov
    
Answer should be:
   
     "Hello warnov here is a complex number 3+5i. And the text content is: Hello World!!!." 


The latest version of this repository contains a testing project called `DllLocalFileFunctionClient`. A simple console application in which you can see how to call the Azure Function from a typical .NET application.


After this, you can publish your function to Azure. Instructions on how to publish from Visual Studio are shown [here](https://blogs.msdn.microsoft.com/webdev/2016/12/01/vis
ual-studio-tools-for-azure-functions/).