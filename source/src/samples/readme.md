# Smartbot Deployment

## Running the app i Heroku using containers:

* Create an app in Heroku
* Either setup a GitHub integration or use the Heroku Git integration
* Set the Heroku stack to be used to containers via `$ heroku stack:set container` (NB, can only be done via the Heroku CLI)
* App config: For Slack notifications, setup a Slack app providing a token with access to post messages. The token should be stored in an env variable `JorgBot_SlackApiKey_SlackApp`. This can done via the Heroku apps settings tab under _Config vars_.
* `git push` to either GitHub / Heroku to trigger deployments.
* To view logs, run `heroku logs` (optionally add `-t` for tail).

Gotchas:
* The Dockerfile needs a `CMD` step as opposed to `ENTRYPOINT`. Not sure why one over the other, but [according to the docs](https://devcenter.heroku.com/articles/build-docker-images-heroku-yml#build-defining-your-build) Heroku will look for a `CMD` to run if you don't specify a `run`in the `heroku.yml`.
