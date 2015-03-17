Castle.RabbitMq
===============





Differences between Castle.RabbitMq and EasyNetQ
------------------------------------------------

Castle.RabbitMq will not, under any circumstance, infer or suggest queue, exchanges names or routing keys based on conventions or any other way. It expects those to be set explicitly in the API usage. The reason is twofold:

# We want to maximize interoperability with systems built in other languages
# We want to be refactoring friendly (changing a namespace or type name will have no effect)

You can build conventions and whatnot on top of Castle.RabbitMq, but that's up to you. 


Using it
--------






How to help
-----------

Send a pull request


Contact
-------

Check [Castle's Get Involved](http://www.castleproject.org/get-involved/mailing-lists/) page. 

