# web-sql

This is an IIS HTTP handler to execute SQL commands via web requests.

For now it supports calling stored procedures with parameters passed as JSON-structured data 
and returning one or more row sets also as JSON-structured data.

## Usage

### The Basics

Include the stored procedure name as the last part of the URL path, like "http://my_server/my_application/.../my_procedure". 
The exact form of ther complete URL must match the mapping defined for this handler in the site's `web.config` file.

Pass the procedure parameters as a JSON object whose property names match the parameter names, for instance:
```JSON
{
  "StringParameter":"A string value",
  "IntegerParameter":33,
  "DecimalParameter":123.45
}
```

Expect the response in the form of an array of arrays of rows (a stored procedure may return several row sets), wrapped into a JSON object
to indicate whether the call has succeeded or not. A successful operation result looks like this (two sets of 3 and 1 row, respectively):
```JSON
{
  "data":[
    [
      {"name":"John","age":33},
      {"name":"Mary","age":35},
      {"name":"Simon","age":23},
    ],
    [
      {"ten":10,"pi":3.1416}
    ]
  ]
}
```

A failed operation result looks like this (multiple errors are shown if linked by their `InnerException` property):
```JSON
{
 "error":[
    {
      "type": "System.Exception",
      "message": "Procedure 'my_procedure' not found"
    }
  ]
}
```

### Authentication

The handler does not have its own means to authenticate the requests; instead it relies on the standard IIS authentication methods.

When serving non-anonymous calls, the handler can provide the caller identity to the procedure being called. In order to do so,
the caller must include that parameter in the request data, but instead of providing an actual parameter value, an object must be passed 
having the **exact** content `{"kind":"Property","name":"RequestIdentityName"}`.

*(this intricate syntax is due to the intention to further develop this feature, of passing as arguments more information specific to
the web context instead of data managed by the caller)* 

## Building and configuration

### Building

The published files can be used to download a Visual Studio 2015 solution ready to use for building the handler under .Net Framework 4.5.2. 
With small changes the handler cam be built under earlier versions of .Net Framework; however, more changes are necessary for versions earlier than 3.5
due to intensive usage of Linq-related functions.  

For JSON serialization and deserialization the handler uses the [Newtonsoft.Json](https://www.newtonsoft.com/json) library.

### Installation

The libraries (Avat.WebSqlHandler.dll and Newtonsoft.Json.dll) must be accessible to the IIS web site using the handler; the most popular 
way to ensure this is having the files in the `\bin` folder of the site.

The handler must be declared in the `web.config` file of the site and associated with a request path pattern, for instance:
```XML
<system.webServer>
  <handlers>
    <add name="WebSqlHandler" path="/sql/*" verb="*" type="Avat.WebSql.Handler" resourceType="Unspecified" preCondition="integratedMode" />
  </handlers>
</system.webServer>
```
*(In this case, the handler would be passed all requests where the path starts with "sql/")*

The SQL connection may be of any type (as long as the underlying database supports stored procedures) and should be declared in the `web.config` file
of the site, under "connection strings", along with proper SQL provider identification **and named 'db'**, for instance:
```XML
<connectionStrings>
  <add connectionString="<adequate connection string>" name="db" providerName="adequate provider name" />
</connectionStrings>
```

**NOTE:** The IIS Manager GUI can be used to declare the handler, but the connection string declaration it generates may be incomplete 
(missing provider information) so editing the `web.config` file directly may be safer.

## Test Client

The "TestClient" project within the solution builds a console program that can be used to test a web site that uses the handler.
Request data should be provided as a text file, while the response is shown in a verbose manner. 
 

