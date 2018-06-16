# api-skeleton
Output the skeleton of a project...

This is a simple project to simulate some Rosyln operations. The console app. basiclly checks a given \*.sln path and with given project name. The projects are a "Legacy" app. which have some defination rules. 

Every class should be implemented from a generic class such as SomeClass<IBusiness, BaseEntity>. So this small app. just checks if every class in a project is implemented from this generic type. If they are implemented from that type, also some members' info(methods and properties) are displayed. 

With this approach, the skeleton of the "Legacy" application can be easly determined. There were some business related conditions while checking these types, I have to remove them to share this approach. 

But the main idea is to show, how to check and dig more for code structures with Rosyln. 
