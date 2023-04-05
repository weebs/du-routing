# du-routing
An example of how to emulate a piece of OCaml's GADTs functionality using F#'s ADTs and generic constraints. This allows you to create a function who's return type is based upon the union case given as an argument, and is known at compile time, using the help of an intermediate "fetch" function.

This can be used for emulating function overloading in modules, or as a way to communicate between two processes with a defined API that includes response types.
