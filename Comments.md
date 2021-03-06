# My comments about ProductApplicationService

## Top 5 areas of concern
* Return value for `ConfidentialInvoiceDiscount` and `BusinessLoans` was calculated by a very strange (and duplicated) code,
  which messed `ApplicationId` with success flag, in such away, so unsuccessfull application 
  is indifferable (just `-1`) from an absent `ApplicationId`. It looks very strange, looks like an error, 
  but I'm not sure, what the initial intent was. In my opinion, it's much better to use 
  the same `IApplicationResult` with quite universal and detailed information about success, errors, and `ApplicationId` (if it's present).
  * Fixed somehow, but should be discussed about initial intents and requirements.
* `ProductApplicationService.SubmitApplicationFor` returns value of `int`, and it's very non-ideomatic, looks more like from old C. Maybe it was inspired by ugly `ISelectInvoiceService` legacy inteface, but that isn't a good reason to keep it.
  * Fixed. Our own `PlainCodeApplicationResult` was added, implementing fairly good `IApplicationResult` interface.

## Other comments
* Throwing `InvalidOperationException` without any message leads to difficult production maintenance.
  * Fixed
* There are no arguments check against `null`. It's very reasonable to have them even with C# 8.
  * Fixed
* `CompanyData.Number.ToString()` was called without culture specification. It's a little bit dangerous.
  * Fixed.
* `IApplicationResult` allows to change it's properties, without sensible reasons for me.
  * I would like to change it to read only, but you asked me not to refactor External 
* Minor readability: request formation statements was embedded into the call statements, that's difficult to read/maintain.'
  * Fixed.
* `CompanyDataRequest` composing logic was duplicated.
  * Fixed.
* It's not clear why `IProduct` interface was declared with `Id`, nobody uses product's `Id`.
  Actually it is used only as a "union type" for completely different data structures.
  * Removed.
* It's not clear why a lot of interfaces declared (`ISellerApplication`, `ISellerCompanyData`, etc) for simple data structures (DTO). 
  These interfaces are not used now neither in code, nor in tests.
  At the moment it's just a useless boilerplate code.
  * Removed.
* Single large dispatching/adapting method was too complex and had a tend to enlarge even more in the future
  * Fixed. Dispatching logic was separated from adapting logic. At the same time, IMHO, it's too early to separate that simple logic into different classes.




