# Task implementation notes

## Introduction

Even though on the surface this project looks simple, there is a huge amount of details to consider when implementing it. While some decisions are rather straightforward, a lot of others require considering many alternatives. Oftentimes, there is no single best solution for a situation, but different solutions with different sets of pros and cons.

If You see something unexpected or weird in the implementation, there is probably a reason for that. I would be happy to answer any questions that You might have, so please feel free to reach out!

## Architecture and code structure

## Future considerations and potential improvements

The following is a list of various things to consider and improve, especially since the codebase is a constantly evolving thing:

* The deletion of a cover should handle the deletion of related claims in one of the following ways, because a claim without a cover cannot exist on it's own (it's a child the claim):
  * All child claims should simply be deleted, if any;
  * An exceptions should be raised if at least one claim is attached to a cover.
