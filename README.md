# Codenet.Dojo
Public repository for the Codenet Dojo kata-processing server.

### Simple Overview

![simple overview](http://i.imgur.com/NFW8kpw.png)

Two strings are passed to the service method `ProcessSimple(string code, string tests)`
The code is compiled to an a `byte[]` and used to create a MetadataReference to the tests.
Once the tests are compiled, we handle the `AppDomain.AssemblyResolve` event to hand the code assembly to the tests.  It's a bit of a hack, but for now it works
If for some reason the code or tests don't compile, we should capture that information and pass it back in a readable format.
Otherwise, we find all the instances of  `[TestClass]` and all instances of `[TestMethod]` within those classes and run the tests.
If the tests fail, we catch the exceptions and format them to be displayed to the user.
Otherwise, we return that all tests passed (probably a message for each test).

There are some gaping holes in the description of what this should do and how it should work, but this is where we want to head next.  Once we've reached this point, we'll revisit design and pursue the next milestone.
