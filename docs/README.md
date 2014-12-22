# ReactiveUI Documentation

The source for ReactiveUI documentation is here under `sources/` and uses extended
Markdown, as implemented by [MkDocs](http://mkdocs.org).

These files are built and deployed to [docs.reactiveui.net](http://docs.reactiveui.net)
whenever `master` is updated.

## Contributing

### Pre-requisites

On Windows, you should use
[Chocolatey](http://chocolatey.org/) to setup your local environment:

> cinst python2
> cinst pip

On OS X, you should use [Homebrew](http://brew.sh/):

> brew install python

Once you've done that, install [MkDocs](http://mkdocs.org) from the command line:

> pip install mkdocs

### Build and Test

Run this command from the root of the ReactiveUI repository:

> mkdocs serve

And point your browser to `http://localhost:8000` to view the documentation running
locally. As files are changed, MkDocs will rebuild the documentation in the
background.