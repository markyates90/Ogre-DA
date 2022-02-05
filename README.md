# Ogre-DA
A layered data access library that starts with a simple abstraction of ADO.Net, adds a light ORM and defines a Fluent Query Language. 

# February 5, 2022
Updated to .Net 6
This simplied the project structure removing the need for full framework and core versions of some files. Also removed the dependency on a config file. The connection information now need to be passed to the Database object on instantiation instead of a ConnectionString key name.