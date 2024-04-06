# Task implementation notes

## Introduction

Even though on the surface this project looks simple, there is a huge amount of details to consider when implementing it. While some decisions are rather straightforward, a lot of others require considering many alternatives. Oftentimes, there is no single best solution for a situation, but different solutions with different sets of pros and cons.

If You see something unexpected or weird in the implementation, there is probably a reason for that. I would be happy to answer any questions that You might have, so please feel free to reach out!

## Architecture and code structure

## Future considerations and potential improvements

The following is a list of various things to consider and improve, especially since the codebase is a constantly evolving thing:

* **Resource ownership**. Database and queue ownership issues: TODO
* **Database handling in tests**. API tests delete the main Cosmos DB database before each test is run. A few related considerations:
  * If the tests successfully creates the database via the API initialization, the database will still remain created and will be in the state of the last test that ran. This aids in debugging that test and might be considered desirable for that reason. If that is undesirable, the clean-up could be moved to after each test, or after all of them (more on that below);
  * API tests are rather slow in general and the additional step of deleting the database for each test makes them even slower. The main reason to keep it that way is to keep each test isolated from each other. An alternative approach could be use where the database deletion occurs just once for all the API tests. This would improve performance, but it would require changes to most of the tests. Most likely, they would need to be made ordered, so that each test knows the exact state of the database. Though there would be a benefit of simplifying certain tests, where multiple endpoints interract. For example, to get a cover via HTTP GET, one needs to create it via an HTTP POST. If tests are isolated (the database gets deleted for each test), the cover creation POST calls might need to be added to each test that later uses that created cover. But if tests are interconnected (a single database delete per all tests in a batch), an earlier test might populate the database with a cover that the later test get then retrieve.
* **Validation**. The deletion of a cover should handle the deletion of related claims in one of the following ways, because a claim without a cover cannot exist on it's own (it's a child the claim):
  * All child claims should simply be deleted, if any;
  * An exceptions should be raised if at least one claim is attached to a cover.
* **Minimal API**. It might be desirable to replace controllers with the minimal API of ASP.NET due the following reasons (which also tend to apply to application service classes):
  * On a more philosophical level, I would argue that controllers do not represent any "real" concept in programming. Ideally, all names in the code should belong to the domain that they represent (in here, "domain" is a more broader term than the application domain; the domain can be that of data structures with its queues, arrays, etc.; or the domain can be that of databases with its tables, columns, rows, etc.). The term "controller" is a rather vague name and does not reflect it's purpose. In addition, it acts more as namespace for controller actions, rather than a "real" object. When an HTTP call is made to an API, the related controller gets instantiated and a single method on it gets called. In general, when using a "real" object (if well designed) it is normal to call multiple methods on a single instance of an object during the object's lifetime.
  * In more practical terms, controllers tend to attract more dependencies with each new controller action. But, more importantly, the dependencies can become injected only because a single action might need it.