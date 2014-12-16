# ReactiveUI Documentation

The source for ReactiveUI documentation is here under `sources/` and uses extended
Markdown, as implemented by [MkDocs](http://mkdocs.org).

The HTML files are built and hosted on `http://docs.reactiveui.net`, and update
automatically after each change to the master branch of [ReactiveUI on
GitHub](https://github.com/ReactiveUI/ReactiveUI) thanks to post-commit hooks. 

## Contributing

Be sure to follow the [contribution guidelines](../CONTRIBUTING.md).
In particular, [remember to sign your work!](../CONTRIBUTING.md#sign-your-work)

## Getting Started

ReactiveUI documentation builds are done in a ReactiveUI container, which installs all
the required tools, adds the local `docs/` directory and builds the HTML docs.
It then starts a HTTP server on port 8000 so that you can connect and see your
changes.

In the root of the `ReactiveUI` source directory:

    $ make docs
    .... (lots of output) ....
    $ ReactiveUI run --rm -it  -e AWS_S3_BUCKET -p 8000:8000 "ReactiveUI-docs:master" mkdocs serve
    Running at: http://0.0.0.0:8000/
    Live reload enabled.
    Hold ctrl+c to quit.

If you have any issues you need to debug, you can use `make docs-shell` and then
run `mkdocs serve`

## Adding a new document

New document (`.md`) files are added to the documentation builds by adding them
to the menu definition in the `docs/mkdocs.yml` file.

