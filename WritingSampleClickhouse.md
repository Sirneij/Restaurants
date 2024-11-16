# CryptoFlow: Building a secure and scalable system with Axum and SvelteKit - Part 0

## Introduction

Being a pragmatist, it's always intriguing to learn by building cool systems with the best tools currently available. The last time I did this, three series came out and it was fulfilling. Having had some break, I decided to build something with [axum][1], "a rust web application framework that focuses on ergonomics and modularity". As usual, [SvelteKit][2] comes to our rescue at the front end. What are we building? That's a nice question!

We will be building `CryptoFlow`, a Q&A web service like [Stack Overflow][3] where a securely authenticated user can ask question(s) related to the world of cryptocurrency and others (securely authenticated too) can proffer answers. Another cool stuff about it is that on each question page, there will be "real-time" prices and price histories of all the tagged cryptocurrencies (limited to 4 tags per question). The home page may have some charts too. The data used were gotten from [CoinGecko API service][4] (if you use my referral code, `CGSIRNEIJ`, for a subscription I get some commissions). Authentication will be session-based and we'll have modular configuration with robust error handling.

The overall features of `CryptoFlow` include but are not limited to:

- **Secured and Scalable Session-Based Authentication System:** Featuring robust security protocols, including email verification to ensure user authenticity. This system is designed to scale efficiently as user traffic increases.
- **Ergonomic and Modular CRUD Service for Optimal Performance:** The system boasts an intuitive CRUD (Create, Read, Update, Delete) service, ensuring smooth and efficient data management. This is complemented by optimized performance, catering to high-speed interactions and data processing.
- **Modularized Code Structure with Self-Contained Components:** The architecture of `CryptoFlow` is based on a modular design inherited from the [previous series with actix-web][0], promoting maintainability and ease of updates. Each module is self-contained, reducing dependencies and enhancing the system's robustness.
- **Real-Time Cryptocurrency Data Integration:** Leveraging the [CoinGecko API][4](don't forget to use my code `CGSIRNEIJ` for your subscription), the platform provides real-time cryptocurrency data, enriching user experience with up-to-date market insights.
- **Secure and Efficient Data Storage:** Utilizing modern database solutions (PostgreSQL & Redis), `CryptoFlow` ensures secure and efficient storage and retrieval of data, maintaining high standards of data integrity and accessibility.

> **NOTE: The program isn't feature-complete yet! Contributions are welcome.**

A handful of the ideas come from the [previous series][0] with some slight modifications for better modularity. Error handling was also more robust. Without further ado, let's get into it!

## Technology stack

For emphasis, our tech stack comprises:

- Backend - Some crates that will be used are:

  - [Axum v0.7][1] - Main backend web framework
  - [tokio][6] - An asynchronous runtime for Rust
  - [serde][7] - Serializing and Deserializing Rust data structures
  - [MiniJinja v1][8] - Templating engine
  - [SQLx][10] - Async SQL toolkit for rust
  - [PostgreSQL][9] - Database
  - [Redis][5] - A storage to store tokens, and sessions etc.
  - [Docker][13] - For containerization

- Frontend - Some tools that will be used are:
  - [SvelteKit][3] - Main frontend framework
  - [Typescript][11] - Language in which the frontend will be written
  - Pure CSS3 and [TailwindCSS v3.4][12] - Styles
  - HTML5 - Structure

## Assumption

A simple prerequisite is to skim through the [previous series][0].

## Source code

The source code for this series is hosted on GitHub via:

[github.com/Sirneij/cryptoflow][100]

## Project structure

Currently, the `backend` structure looks like this:

```shell
.
├── Cargo.lock
├── Cargo.toml
├── migrations
│   ├── 20231230194839_users_table.down.sql
│   ├── 20231230194839_users_table.up.sql
│   ├── 20240101210638_qanda.down.sql
│   └── 20240101210638_qanda.up.sql
├── settings
│   ├── base.yml
│   ├── development.yml
│   └── production.yml
├── src
│   ├── lib.rs
│   ├── main.rs
│   ├── models
│   │   ├── mod.rs
│   │   ├── qa.rs
│   │   └── users.rs
│   ├── routes
│   │   ├── crypto
│   │   │   ├── coins.rs
│   │   │   ├── mod.rs
│   │   │   ├── price.rs
│   │   │   └── prices.rs
│   │   ├── health.rs
│   │   ├── mod.rs
│   │   ├── qa
│   │   │   ├── answer.rs
│   │   │   ├── answers.rs
│   │   │   ├── ask.rs
│   │   │   ├── mod.rs
│   │   │   └── questions.rs
│   │   └── users
│   │       ├── activate_account.rs
│   │       ├── login.rs
│   │       ├── logout.rs
│   │       ├── mod.rs
│   │       └── register.rs
│   ├── settings.rs
│   ├── startup.rs
│   ├── store
│   │   ├── answer.rs
│   │   ├── crypto.rs
│   │   ├── general.rs
│   │   ├── mod.rs
│   │   ├── question.rs
│   │   ├── tag.rs
│   │   └── users.rs
│   └── utils
│       ├── crypto.rs
│       ├── email.rs
│       ├── errors.rs
│       ├── middleware.rs
│       ├── mod.rs
│       ├── password.rs
│       ├── qa.rs
│       ├── query_constants.rs
│       ├── responses.rs
│       └── user.rs
└── templates
    └── user_welcome.html
```

I know it's overwhelming but most of these were explained in the [previous series][0] and we will go over the rationale behind them as this series progresses.

## Implementation

### Step 1: Start a new project and install dependencies

We'll be using `cryptoflow` as the root directory of both the front- and back-end applications. In the folder, do:

```shell
~/cryptoflow$ cargo new backend
```

As expected, a new directory with `backend` as a name gets created. It comes with `Cargo.toml`, `Cargo.lock`, and `src/main.rs`. Open up the `Cargo.toml` file and populate it with:

```toml
[package]
name = "backend"
version = "0.1.0"
authors = ["John Idogun <sirneij@gmail.com>"]
edition = "2021"

[lib]
path = "src/lib.rs"

[[bin]]
path = "src/main.rs"
name = "backend"

[dependencies]
argon2 = "0.5.2"
axum = { version = "0.7.3", features = ["macros"] }
axum-extra = { version = "0.9.1", features = ["cookie-private", "cookie"] }
bb8-redis = "0.14.0"
config = { version = "0.13.4", features = ["yaml"] }
dotenv = "0.15.0"
itertools = "0.12.0"
lettre = { version = "0.11.2", features = ["builder", "tokio1-native-tls"] }
minijinja = { version = "1.0.10", features = ["loader"] }
pulldown-cmark = "0.9.3"
regex = "1.10.2"
reqwest = { version = "0.11.23", features = ["json"] }
serde = { version = "1.0.193", features = ["derive"] }
serde_json = "1.0.108"
sha2 = "0.10.8"
sqlx = { version = "0.7.3", features = [
    "runtime-async-std-native-tls",
    "postgres",
    "uuid",
    "time",
    "migrate",
] }
time = { version = "0.3.31", features = ["serde"] }
tokio = { version = "1.35.1", features = ["full"] }
tower-http = { version = "0.5.0", features = ["trace", "cors"] }
tracing = "0.1.40"
tracing-subscriber = { version = "0.3.18", features = ["env-filter"] }
uuid = { version = "1.6.1", features = ["v4", "serde"] }
```

The prime suspects are `axum`, `argon2` (for password hashing), `axum-extra` (used for cookie administration), `bb8-redis` (an async redis pool), `pulldown-cmark` (for converting markdown to HTML), and others.

### Step 2: Build out the project's skeleton

Building out the skeletal structure of the project is the same as what we had in the [actix-web version][0] apart from the absence of `src/telemetry.rs`:

```shell
~/cryptoflow/backend$ touch src/lib.rs src/startup.rs src/settings.rs

~/cryptoflow/backend$ mkdir src/routes && touch src/routes/mod.rs src/routes/health.rs
```

`src/lib.rs` and `src/settings.rs` have almost same content. The updated part of `src/settings.rs` is:

```rust
// src/settings.rs
#[derive(serde::Deserialize, Clone)]
pub struct Secret {
    pub token_expiration: i64,
    pub cookie_expiration: i64,
}

/// Global settings for exposing all preconfigured variables
#[derive(serde::Deserialize, Clone)]
pub struct Settings {
    pub application: ApplicationSettings,
    pub debug: bool,
    pub email: EmailSettings,
    pub frontend_url: String,
    pub interval_of_coin_update: u64,
    pub superuser: SuperUser,
    pub secret: Secret,
}
```

Based on this update, the `.yml` files have:

```yml
# settings/base.yml
---
interval_of_coin_update: 24
```

There is a background task we will write that periodically fetches the updated list of coins from [CoinGecko API][4]. The interval with which the task runs (in hours) is what `interval_of_coin_update` holds.

```yml
# settings/development.yml
---
secret:
  token_expiration: 15
  cookie_expiration: 1440
```

Those settings do exactly what their names imply. They store the expiration periods (in minutes) of the token and cookie respectively.

Next is `src/startup.rs`:

```rust
use crate::routes;

pub struct Application {
    port: u16,
}

impl Application {
    pub fn port(&self) -> u16 {
        self.port
    }
    pub async fn build(
        settings: crate::settings::Settings,
        _test_pool: Option<sqlx::postgres::PgPool>,
    ) -> Result<Self, std::io::Error> {
        let address = format!(
            "{}:{}",
            settings.application.host, settings.application.port
        );

        let listener = tokio::net::TcpListener::bind(&address).await.unwrap();
        let port = listener.local_addr().unwrap().port();

        tracing::info!("Listening on {}", &address);

        run(listener, settings).await;

        Ok(Self { port })
    }
}

async fn run(
    listener: tokio::net::TcpListener,
    settings: crate::settings::Settings,
) {
    // build our application with a route
    let app = axum::Router::new()
        .route(
            "/api/health-check",
            axum::routing::get(routes::health_check),
        )
        .layer(tower_http::trace::TraceLayer::new_for_http());

    axum::serve(listener, app.into_make_service())
        .with_graceful_shutdown(shutdown_signal())
        .await
        .unwrap();
}

async fn shutdown_signal() {
    let ctrl_c = async {
        signal::ctrl_c()
            .await
            .expect("failed to install Ctrl+C handler");
    };

    #[cfg(unix)]
    let terminate = async {
        signal::unix::signal(signal::unix::SignalKind::terminate())
            .expect("failed to install signal handler")
            .recv()
            .await;
    };

    #[cfg(not(unix))]
    let terminate = std::future::pending::<()>();

    tokio::select! {
        _ = ctrl_c => {},
        _ = terminate => {},
    }
}
```

The `build` method is similar to the one in [this series][0]. What interests us is the content of the `run` function. [Axum][1] uses `Router` to channel requests and you could have a simple route like `/api/health-check` and all legitimate requests to that URL will be handled by the "handler" you point it to. Handlers are asynchronous functions which can take request extractors as arguments while returning responses to the client. The response must be convertible into a response. An example of a handler is the `health_check` located in `src/routes/health.rs`:

```rust
use crate::utils::SuccessResponse;
use axum::{http::StatusCode, response::IntoResponse};

#[tracing::instrument]
pub async fn health_check() -> impl IntoResponse {
    SuccessResponse {
        message: "Rust(Axum) and SvelteKit application is healthy!".to_string(),
        status_code: StatusCode::OK.as_u16(),
    }
    .into_response()
}
```

This handler doesn't take an extractor but its response implements the `IntoResponse`. The `SuccessResponse` does implement it in `src/utils/responses.rs`:

```rust
use axum::{
    http::StatusCode,
    response::{IntoResponse, Response},
};
use serde::Serialize;

#[derive(Serialize)]
pub struct SuccessResponse {
    pub message: String,
    pub status_code: u16,
}

impl IntoResponse for SuccessResponse {
    fn into_response(self) -> Response {
        let status = StatusCode::from_u16(self.status_code).unwrap_or(StatusCode::OK);
        let json_body = axum::Json(self);

        // Convert Json to Response
        let mut response = json_body.into_response();

        // Set the correct status code
        *response.status_mut() = status;

        response
    }
}
```

I have a struct that is serializable and I implemented `IntoResponse` for it. The `into_response` method uses `axum::Json` to serialize the struct into `JSON` which uses its `into_response()` to create a response. Since I wanted to be able to state the status code after each call, I used the `status_mut` to do this. You don't have to do it this way though.

You also get to specify the accepted HTTP method of the URL via `axum::routing`. To answer its name, modularity, [Axum][1] also supports nested routes as we'll see later in this series. Next is the `layer`, a method used to apply ` tower::Layer` to all routes before it. This means that routes added after the `layer` method will not have such a layer applied to their requests. In our case, we used the layer to add tracing to all HTTP requests and responses to our routes. This is needed for proper logging. The `tower_http::trace::TraceLayer` can even be really [customised][14].

Having created an app instance, it's now left to serve it. In this case and in axum version 0.7, we used the `serve` method to supply the address we want our app to listen to for requests and also ensure we have a graceful shutdown of the application. Graceful shutdown was introduced in the latest version and it's quite nifty! If you go through my series on [building stuff with go][15], I explained more about the graceful shutdown of application processes. Especially concurrent applications that spawn many threads like our application. The code for the shutdown signal was taken from this [example][16].

With that explained, our application can't still serve any requests. This is because Rust's applications have `main` as their entry points. We also need a `main` function to serve as our application entry point. Therefore, edit `src/main.rs` to be like this:

```rust
use tracing_subscriber::{layer::SubscriberExt, util::SubscriberInitExt};

#[tokio::main]
async fn main() -> std::io::Result<()> {
    dotenv::dotenv().ok();

    let settings = backend::settings::get_settings().expect("Failed to read settings.");

    let subscriber_format = if settings.debug {
        tracing_subscriber::EnvFilter::try_from_default_env().unwrap_or_else(|_| {
            "backend=debug,tower_http=debug,axum::rejection=debug,sqlx=debug".into()
        })
    } else {
        tracing_subscriber::EnvFilter::try_from_default_env().unwrap_or_else(|_| {
            "backend=info,tower_http=info,axum::rejection=info,sqlx=info".into()
        })
    };

    tracing_subscriber::registry()
        .with(subscriber_format)
        .with(tracing_subscriber::fmt::layer())
        .init();

    backend::startup::Application::build(settings, None).await?;

    Ok(())
}
```

Axum uses tokio as a runtime and it was built on top of it. It is also being maintained by the tokio team with David Peterson, the creator, leading the park. Therefore, our application uses the tokio runtime exclusively! In the function, we first bring our `.env` into the application state using `dotenv`. We then loaded the settings we defined before now. Using the `debug` attribute of the `Settings` struct, we state how deep the tracing or logging should be for our application(`backend`), `tower_http` and `sqlx`. For production, we will change the format of the logs to `JSON` for easy parsing. Lastly, we used the `build` method defined in `src/startup.rs` to serve the application. It's time to test it out!

Open up your terminal and issue the following command:

```shell
~/cryptoflow/backend$ cargo watch -x 'run -- --release'
```

I used [cargo-watch][17] here so that every time my source changes, the server will automatically restart and re-serve the updated code.

If you navigate to your browser now and visit `http://127.0.0.1:8080/api/health-check`, you should see something like:

```json
{
  "message": "Rust(Axum) and SvelteKit application is healthy!",
  "status_code": 200
}
```

Yay!!! It's awesome!

## Step 3: Project's SQL relations

As stated, `CryptoFlow` will support user authentication and authorization and questions and answers management. We need to persist these data and to do this, we need SQL relations. We're building the application with [PostgreSQL][9]. Since we use `SQLx`, our data schema needs to be inside the `migrations` folder at the root of of project (in the same level as `src`). Create this folder and then issue the following command:

```shell
~/cryptoflow/backend$ sqlx migrate add -r users_table
```

This will create two `.sql` files inside the `migrations` folder due to the `-r` argument (if you don't want two files, you can effectively omit the argument). One of the files, `.up.sql`, should have the table relations while `.down.sql` should be able to reverse what `.up.sql` file does effectively.

The contents of the files are:

```sql
-- migrations/*_users_table.down.sql
-- Add down migration script here
DROP TABLE IF EXISTS users;
```

```sql
-- migrations/*_users_table.up.sql
-- Add up migration script here
CREATE TABLE IF NOT EXISTS users(
    id UUID NOT NULL PRIMARY KEY DEFAULT gen_random_uuid(),
    email TEXT NOT NULL UNIQUE,
    password TEXT NOT NULL,
    first_name TEXT NOT NULL,
    last_name TEXT NOT NULL,
    is_active BOOLEAN DEFAULT FALSE,
    is_staff BOOLEAN DEFAULT FALSE,
    is_superuser BOOLEAN DEFAULT FALSE,
    thumbnail TEXT NULL,
    date_joined TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX IF NOT EXISTS users_id_is_active_indx ON users (id, is_active);
CREATE INDEX IF NOT EXISTS users_email_is_active_indx ON users (email, is_active);
```

As can be seen, we created a `relation`, `users`, with 10 `attributes` (columns) with various `domains` (data types). A user should have a unique identification and an `email`. For authentication, the user's HASHED `password` should also be saved (**BEWARE of saving plain password!!**). We also want to have basic user information such as `first_name`, `last_name`, and `thumbnail` (user's picture). For authorization, we have three flags:

- `is_active`: Just registering on our application is not enough as there might be some malicious users who use emails that don't exist to do that. When a user registers, we default this field to `false`. As soon as a user correctly provides a token sent to their email, we set it to `true`. Before you can login into our app, you MUST be an active user!
- `is_staff`: In a typical system serving a company, there are some elevated users who may be the company's members of staff who have certain privileges above the users of the company's product. This field helps to distinguish them.
- `is_superuser`: This is reserved for the creator of the company or application. Such a person should have pretty great access to things. This shouldn't be abused though.

We also created indexes for the table. Since we'll be filtering the `users` frequently with either `id` and `is-active` or `email` and `is_active` combination, it's good to have them as indexes to facilitate getting them quickly by minimizing I/O operations and catalyzing efficient access to the storage disk. A properly indexed lookup (using [B+-tree][0] which most DBMSs use) is fast ({% katex inline %}O(log(N)){% endkatex %} for {% katex inline %}N{% endkatex %} `tuples` (rows)). It comes at a cost though. Inserting data into the table will be impacted a bit.

Next is the `qanda` relation:

```shell
~/cryptoflow/backend$ sqlx migrate add -r qanda
```

```sql
-- migrations/*_qanda.down.sql
-- Add down migration script here
DROP TABLE IF EXISTS question_tags;
DROP TABLE IF EXISTS tags;
DROP TABLE IF EXISTS answers;
DROP TABLE IF EXISTS questions;
DROP FUNCTION IF EXISTS trigger_set_timestamp();
```

```sql
-- migrations/*_qanda.up.sql
-- Add up migration script here
-- Trigger function to update the timestamp on the 'questions' table
CREATE OR REPLACE FUNCTION update_questions_timestamp() RETURNS TRIGGER AS $$ BEGIN NEW.updated_at = NOW();
RETURN NEW;
END;
$$ LANGUAGE plpgsql;
-- Trigger function to update the timestamp on the 'answers' table
CREATE OR REPLACE FUNCTION update_answers_timestamp() RETURNS TRIGGER AS $$ BEGIN NEW.updated_at = NOW();
RETURN NEW;
END;
$$ LANGUAGE plpgsql;
-- Questions table
CREATE TABLE IF NOT EXISTS questions (
    id UUID NOT NULL PRIMARY KEY DEFAULT gen_random_uuid(),
    title TEXT NOT NULL,
    slug TEXT NOT NULL UNIQUE,
    content TEXT NOT NULL,
    raw_content TEXT NOT NULL,
    author UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX IF NOT EXISTS questions_index_title ON questions (title, slug);
CREATE TRIGGER update_questions_timestamp BEFORE
UPDATE ON questions FOR EACH ROW EXECUTE PROCEDURE update_questions_timestamp();
-- Answers table
CREATE TABLE IF NOT EXISTS answers (
    id UUID NOT NULL PRIMARY KEY DEFAULT gen_random_uuid(),
    content TEXT NOT NULL,
    raw_content TEXT NOT NULL,
    author UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    question UUID NOT NULL REFERENCES questions(id) ON DELETE CASCADE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE TRIGGER update_answers_timestamp BEFORE
UPDATE ON answers FOR EACH ROW EXECUTE PROCEDURE update_answers_timestamp();
-- Tags table
CREATE TABLE IF NOT EXISTS tags (
    id VARCHAR(255) NOT NULL PRIMARY KEY,
    name VARCHAR (255) NOT NULL,
    symbol VARCHAR (255) NOT NULL
);
CREATE INDEX IF NOT EXISTS tags_index_name ON tags (name);
CREATE INDEX IF NOT EXISTS tags_index_symbol ON tags (symbol);
-- Question tags table
CREATE TABLE IF NOT EXISTS question_tags (
    question UUID NOT NULL REFERENCES questions(id) ON DELETE CASCADE,
    tag VARCHAR(255) NOT NULL REFERENCES tags(id) ON DELETE CASCADE,
    PRIMARY KEY (question, tag)
);
```

The `.up.sql` is a bit involved unlike what we had previously. The file has two triggers, `update_questions_timestamp` and `update_answers_timestamp`, that automatically update the `updated_at` fields of the `questions` and `answers` relations whenever there is an update. I could use a single trigger function for this but I chose this for clarity. We defined a couple of relations:

- `questions`: This table has 8 attributes that help manage the question(s) our app users create. It references the `users` table with the constraint that if a user, say `a`, authors two questions, say `b` and `c`, `b` and `c` get deleted as soon as `a` gets deleted. That's what `ON DELETE CASCADE` do! `CASCADE` is one of the ForeignKey constraints available in DBMSs. The full options are `NO ACTION | CASCADE | SET NULL | SET DEFAULT`, each having different effects. There `content` and `raw_content` attributes. The former stores the compiled markdown of the question while the latter stores the raw markdown. This will help to edit users' questions.
- `answers`: With 7 attributes, this relation is meant to store answers to users' questions (hence it references the `questions` table using the same ForeignKey constraints as discussed above).
- `tags`: This table stores tags (in our case, coins). The attributes are 3 in number and the data here will be gotten directly from the [CoinGecko API][4]. It will periodically be updated every day.
- `question_tags`: This table is interesting. In that, it has only two attributes, both referencing other tables. The table mirrors [Many-to-many][19] relationship. This is because each question can have multiple tags (limited to 4 in our case, which will be enforced later) and each tag can be used by multiple questions.

## Step 4: Application store

With the schemas designed, we need a modular way to talk to the database. This brings us to create a `store` module that does just that:

```rust
// src/store/general.rs
use sqlx::postgres::{PgPool, PgPoolOptions};

#[derive(Clone, Debug)]
pub struct Store {
    pub connection: PgPool,
}

impl Store {
    pub async fn new(db_url: &str) -> Self {
        match PgPoolOptions::new()
            .max_connections(8)
            .connect(db_url)
            .await
        {
            Ok(pool) => Store { connection: pool },
            Err(e) => {
                panic!("Couldn't establish DB connection! Error: {}", e)
            }
        }
    }
}
```

We defined a "clonable" and "debug-compatible" `Store` store and added a `new` method to aid easy database pool creation. We currently only allow 8 connections but this can be any reasonable number and can be made configurable. With this underway, we can modify our `build` method and allow our application's state to have access to the database pool so that any handler can access and use it:

```rust
// src/startup.rs
...

#[derive(Clone)]
pub struct AppState {
    pub db_store: crate::store::Store,
}

impl Application {
    pub fn port(&self) -> u16 {
        self.port
    }
    pub async fn build(
        settings: crate::settings::Settings,
        test_pool: Option<sqlx::postgres::PgPool>,
    ) -> Result<Self, std::io::Error> {
        let store = if let Some(pool) = test_pool {
            crate::store::Store { connection: pool }
        } else {
            let db_url = std::env::var("DATABASE_URL").expect("Failed to get DATABASE_URL.");
            crate::store::Store::new(&db_url).await
        };

        sqlx::migrate!()
            .run(&store.clone().connection)
            .await
            .expect("Failed to migrate");
        ...
        run(listener, store, settings).await;
        ...
    }
}

async fn run(
    listener: tokio::net::TcpListener,
    store: crate::store::Store,
) {
    ...
    let app_state = AppState {
        db_store: store,
    };
    // build our application with a route
    let app = axum::Router::new()
        ...
        .with_state(app_state.clone())
        ...
        ;

    ...
}
...
```

In the updated code, we defined a clonable `AppState` struct. It needs to be clonable because `with_state` needs it. There are some ways around this though. In the `build` method, we tried to detect which environment (testing or normal) so that we could appropriately fetch the database URL needed to effectively connect to the database. We defined `DATABASE_URL` in our`.env` file and it looks like this:

```text
DATABASE_URL=postgres://<user>:<password>@<host>:<port>/<database_name>
```

Next, since we activated `migrate` feature in our `SQLx` installation, we allowed it to automatically migrate the database. This protects us from manual migration though it has its downside. Without migration, the designed schema won't be affected in our database.

We then passed the store to the `AppState` which gets propagated to the entire application.

That's it for the first article in the series!!! See y'all in the next one.

[0]: https://dev.to/sirneij/series/22690 "Secure and performant full-stack authentication system using rust (actix-web) and sveltekit Series"
[1]: https://docs.rs/axum/latest/axum/ "Axum is a web application framework that focuses on ergonomics and modularity."
[2]: https://kit.svelte.dev/ "web development, streamlined"
[3]: https://stackoverflow.com/
[4]: https://www.coingecko.com/en/api "Explore the API"
[5]: https://redis.io/ "The open source, in-memory data store used by millions of developers as a database, cache, streaming engine, and message broker."
[6]: https://tokio.rs/ "Tokio is an asynchronous runtime for the Rust programming language."
[7]: https://serde.rs/ "Serde is a framework for serializing and deserializing Rust data structures efficiently and generically."
[8]: https://github.com/mitsuhiko/minijinja "MiniJinja is a powerful but minimal dependency template engine for Rust"
[9]: https://www.postgresql.org/ "The World's Most Advanced Open Source Relational Database"
[10]: https://crates.io/crates/sqlx "The Rust SQL Toolkit. An async, pure Rust SQL crate featuring compile-time checked queries without a DSL. Supports PostgreSQL, MySQL, and SQLite."
[11]: https://www.typescriptlang.org/ "TypeScript is JavaScript with syntax for types."
[12]: https://tailwindcss.com/ "A utility-first CSS framework packed with classes like flex, pt-4, text-center and rotate-90 that can be composed to build any design, directly in your markup."
[13]: https://www.docker.com/ "Make better, secure software from the start"
[14]: https://github.com/tokio-rs/axum/blob/main/examples/tracing-aka-logging/src/main.rs "Axum example of customizing tracing"
[15]: https://dev.to/sirneij/series/23239 "Secure and performant full-stack authentication system using Golang and SvelteKit Series"
[16]: https://github.com/tokio-rs/axum/tree/main/examples/graceful-shutdown "Axum graceful shutdown example."
[17]: https://crates.io/crates/cargo-watch "Watches over your Cargo project’s source"
[18]: https://en.wikipedia.org/wiki/B%2B_tree "B+-tree"
[19]: https://blog.devart.com/types-of-relationships-in-sql-server-database.html#many-to-many-relationship "Many-to-many relationship"

# CryptoFlow: Building a secure and scalable system with Axum and SvelteKit - Part 1

## Introduction

In [part 0][20], we laid some solid background in building our proposed system. The system's structure, database schema, and other details were laid bare. This article builds on that foundation.

> **NOTE: The program isn't feature-complete yet! Contributions are welcome.**

## Source code

The source code for this series is hosted on GitHub via:

[github.com/Sirneij/cryptoflow][100]

## Implementation

### Step 1: Cookies and user management

As stated in [part 0][20], our system's authentication is cookie(session)-based. To allow this, we need to set it up. Right from the onset, we installed [axum-extra][21] (with `cookie-private` and `cookie` features enabled) and [tower-http][22] (`cors` feature enabled). These will allow us to achieve our aims. Let's kick start by configuring our app to allow cookies and enable an origin (our frontend app) to directly access the application. To do this, we'll add a `key` attribute to the app's state and make some configurations:

```rust
// backend/src/startup.rs
...
use axum::extract::FromRef;
...

#[derive(Clone)]
pub struct AppState {
    ...
    key: axum_extra::extract::cookie::Key,
}

impl FromRef<AppState> for axum_extra::extract::cookie::Key {
    fn from_ref(state: &AppState) -> Self {
        state.key.clone()
    }
}
...
async fn run(
    listener: tokio::net::TcpListener,
    store: crate::store::Store,
    settings: crate::settings::Settings,
) {
    let cors = tower_http::cors::CorsLayer::new()
        .allow_credentials(true)
        .allow_methods(vec![
            axum::http::Method::OPTIONS,
            axum::http::Method::GET,
            axum::http::Method::POST,
            axum::http::Method::PUT,
            axum::http::Method::DELETE,
        ])
        .allow_headers(vec![
            axum::http::header::ORIGIN,
            axum::http::header::AUTHORIZATION,
            axum::http::header::ACCEPT,
        ])
        .allow_origin(
            settings
                .frontend_url
                .parse::<axum::http::HeaderValue>()
                .unwrap(),
        );

    let app_state = AppState {
        ...
        key: axum_extra::extract::cookie::Key::from(
            std::env::var("COOKIE_SECRET")
                .expect("Failed to get COOKIE_SECRET.")
                .as_bytes(),
        ),
    };
    // build our application with a route
    let app = axum::Router::new()
        ...
        .layer(cors);
    ...
}
...
```

We allowed the basic HTTP methods, certain headers (particularly the authorization), and our frontend application via the [tower-http][22] `CorsLayer`. We want our cookies to be very private so we'll be using [axum_extra::extract::cookie::PrivateCookieJar][23] which requires a `key` for data encryption or to sign the cookies. The key can be generated via the `axum_extra::extract::cookie::Key::generate()` but I chose to generate it from a `512-bit` (64 bytes) cryptographically random string which I saved in `.env`. An example of this is:

```text
// .env
COOKIE_SECRET=3bbefd8d24c89aefd3ad0b8b95afd2ea996e47b89d93d4090b481a091b4e73e5543305f2e831d0b47737d9807a1b5b5773dba3bbb63623bd42de84389fbfa3d1
```

To inform `SignedCookieJar` how to access the key from our `AppState`, we made this `impl`:

```rust
...
impl FromRef<AppState> for axum_extra::extract::cookie::Key {
    fn from_ref(state: &AppState) -> Self {
        state.key.clone()
    }
}
...
```

Now to user data management. For modularity, we'll have a `users` submodule in the `routes` module. The submodule will house all user-related handlers and will expose the `Router` instance specific to user management alone. To achieve this, create two files, `src/routes/users/mod.rs` and `src/routes/users/login.rs`. The former is a special filename used for module organization (I assume you already know this) while the latter houses the handler for user login. Before we write their codes, we need to first write some utilities that will make the codes compile and functional. In the `src/utils` folder, create a `password.rs` file and make it look like this:

```rust
use argon2::{
    password_hash::{rand_core::OsRng, PasswordHash, PasswordHasher, PasswordVerifier, SaltString},
    Argon2,
};

#[tracing::instrument(name = "Hashing user password", skip(password))]
pub async fn hash_password(password: &[u8]) -> String {
    let salt = SaltString::generate(&mut OsRng);
    Argon2::default()
        .hash_password(password, &salt)
        .expect("Unable to hash password.")
        .to_string()
}

#[tracing::instrument(name = "Verifying user password", skip(password, hash))]
pub fn verify_password(hash: &str, password: &[u8]) -> Result<(), argon2::password_hash::Error> {
    let parsed_hash = PasswordHash::new(hash)?;
    Argon2::default().verify_password(password, &parsed_hash)
}
```

There are two functions there:

- `hash_password`: This hashes a plain text using the [argon2][24] hashing algorithm. We used the default settings of [argon2 crate][25] which uses `v19` of `Argon2id` &mdash; memory cost is `19 * 1024` (19 MiB), number of iterations is `2`, and the degree of parallelism is `1`. This is as recommended by [OWASP Password Storage Cheat Sheet][26]. This stringified result of this is what will be saved in the `password` attribute of the `users` relation.
- `verify_password`: A returning user will normally provide the plaintext password used for registration alongside the user's email. This password will need to be "compared" with the saved password to ensure that it's correct. To ascertain this correctness, this function was written.

Next in the list of utilities is error handling. This part was heavily influenced by this [example][27] provided by the axum team.

> **NOTE: The code is just part of the implementation. Check out [this file][28] for the complete code.**

```rust
// src/utils/errors.rs
use crate::utils::CustomAppJson;
use argon2::password_hash::Error as ArgonError;
use axum::{
    extract::rejection::JsonRejection,
    http::StatusCode,
    response::{IntoResponse, Response},
};

use serde::Serialize;

pub enum ErrorContext {
    UnauthorizedAccess,
    InternalServerError,
    BadRequest,
    NotFound,
}

pub enum CustomAppError {
    JsonRejection(JsonRejection),
    DatabaseQueryError(sqlx::Error),
    PasswordHashError(ArgonError),
    RedisError(bb8_redis::redis::RedisError),
    UUIDError(uuid::Error),
    Unauthorized(String),
    InternalError(String),
    BadRequest(String),
    NotFound(String),
    ReqwestError(reqwest::Error),
}

impl IntoResponse for CustomAppError {
    fn into_response(self) -> Response {
        // How we want error responses to be serialized

        #[derive(Serialize)]
        struct ErrorResponse {
            message: String,
            status_code: u16,
        }

        let (status, message) = match self {
            CustomAppError::JsonRejection(rejection) => {
                // This error is caused by bad user input so don't log it
                tracing::error!("Bad user input: {:?}", rejection);
                (rejection.status(), rejection.body_text())
            }
            CustomAppError::DatabaseQueryError(error) => {
                match &error {
                    sqlx::Error::RowNotFound => {
                        tracing::error!("Resource not found: {}", error);
                        (
                            StatusCode::NOT_FOUND,
                            "Resource not found or you are not allowed to perform this operation"
                                .to_string(),
                        )
                    }
                    ...
                }
            }
            CustomAppError::PasswordHashError(error) => match error {
                ArgonError::Password => {
                    tracing::info!("Password mismatch error");
                    (
                        StatusCode::BAD_REQUEST,
                        "Email and Password combination does not match.".to_string(),
                    )
                }
                ...
            },
            ...
        };

        (
            status,
            CustomAppJson(ErrorResponse {
                message,
                status_code: status.as_u16(),
            }),
        )
            .into_response()
    }
}

impl From<JsonRejection> for CustomAppError {
    fn from(rejection: JsonRejection) -> Self {
        Self::JsonRejection(rejection)
    }
}

...

impl From<(String, ErrorContext)> for CustomAppError {
    fn from((message, context): (String, ErrorContext)) -> Self {
        match context {
            ErrorContext::UnauthorizedAccess => CustomAppError::Unauthorized(message),
            ErrorContext::InternalServerError => CustomAppError::InternalError(message),
            ErrorContext::BadRequest => CustomAppError::BadRequest(message),
            ErrorContext::NotFound => CustomAppError::NotFound(message),
        }
    }
}
```

It's some straightforward code that allows many of the expected errors (from `SQLx`, `bb8_redis`, `argon2`, `uuid` and others) to be gracefully handled.

For data extraction from requests' bodies, I also made a simple `JSON` extractor (for now) in `src/utils/responses.rs`:

```rust
use axum::{
    extract::FromRequest,
    http::StatusCode,
    response::{IntoResponse, Response},
};

use crate::utils::CustomAppError;

use serde::Serialize;

#[derive(FromRequest)]
#[from_request(via(axum::Json), rejection(CustomAppError))]
pub struct CustomAppJson<T>(pub T);

impl<T> IntoResponse for CustomAppJson<T>
where
    axum::Json<T>: IntoResponse,
{
    fn into_response(self) -> Response {
        axum::Json(self.0).into_response()
    }
}
...
```

It does what `axum::Json` would do with an extra.

The next utility functions will be creating and retrieving (for now) users from the database. To write them, we will remember the `Store` struct we implemented in [part 0][20]. We will extend it by adding methods that will facilitate those operations:

```rust
// src/store/users.rs
use sqlx::Row;

impl crate::store::Store {
    #[tracing::instrument(name = "get_user_by_id", fields(user_id = id.to_string()))]
    pub async fn get_user_by_id(&self, id: uuid::Uuid) -> Result<crate::models::User, sqlx::Error> {
        sqlx::query_as::<_, crate::models::User>(
            r#"
        SELECT
            id,
            email,
            password,
            first_name,
            last_name,
            is_active,
            is_staff,
            is_superuser,
            thumbnail,
            date_joined
        FROM users
        WHERE id = $1 AND is_active = true
        "#,
        )
        .bind(id)
        .fetch_one(&self.connection)
        .await
    }

    #[tracing::instrument(name = "get_user_by_email", fields(user_email = email))]
    pub async fn get_user_by_email(&self, email: &str) -> Result<crate::models::User, sqlx::Error> {
        sqlx::query_as::<_, crate::models::User>(
            r#"
        SELECT
            id,
            email,
            password,
            first_name,
            last_name,
            is_active,
            is_staff,
            is_superuser,
            thumbnail,
            date_joined
        FROM users
        WHERE email = $1 AND is_active = true
        "#,
        )
        .bind(email)
        .fetch_one(&self.connection)
        .await
    }

    #[tracing::instrument(name = "create_user", skip(password), fields(user_first_name = first_name, user_last_name = last_name, user_email = email))]
    pub async fn create_user(
        &self,
        first_name: &str,
        last_name: &str,
        email: &str,
        password: &str,
    ) -> Result<crate::models::UserVisible, sqlx::Error> {
        sqlx::query_as::<_, crate::models::UserVisible>(
        r#"
        INSERT INTO users (first_name, last_name, email, password)
            VALUES ($1, $2, $3, $4)
        RETURNING
            id, email, first_name, last_name, is_active, is_staff, is_superuser, thumbnail, date_joined
        "#
    )
    .bind(first_name)
    .bind(last_name)
    .bind(email)
    .bind(password)
    .fetch_one(&self.connection)
    .await
    }

    #[tracing::instrument(name = "activate_user", fields(user_id = id.to_string()))]
    pub async fn activate_user(&self, id: &uuid::Uuid) -> Result<(), sqlx::Error> {
        sqlx::query(
            r#"
        UPDATE users
        SET is_active = true
        WHERE id = $1
        "#,
        )
        .bind(id)
        .execute(&self.connection)
        .await?;

        Ok(())
    }

    #[tracing::instrument(name="create_super_user_in_db.", skip(settings), fields(user_email = settings.superuser.email, user_first_name = settings.superuser.first_name, user_last_name = settings.superuser.last_name))]
    pub async fn create_super_user_in_db(&self, settings: &crate::settings::Settings) {
        let new_super_user = crate::models::NewUser {
            email: settings.superuser.email.clone(),
            password: crate::utils::hash_password(&settings.superuser.password.as_bytes()).await,
            first_name: settings.superuser.first_name.clone(),
            last_name: settings.superuser.last_name.clone(),
        };

        match sqlx::query(
            "INSERT INTO users
                    (email, password, first_name, last_name, is_active, is_staff, is_superuser)
                VALUES ($1, $2, $3, $4, true, true, true)
                ON CONFLICT (email)
                DO UPDATE
                    SET
                        first_name=EXCLUDED.first_name,
                        last_name=EXCLUDED.last_name
                RETURNING id",
        )
        .bind(new_super_user.email)
        .bind(&new_super_user.password)
        .bind(new_super_user.first_name)
        .bind(new_super_user.last_name)
        .map(|row: sqlx::postgres::PgRow| -> uuid::Uuid { row.get("id") })
        .fetch_one(&self.connection)
        .await
        {
            Ok(id) => {
                tracing::info!("Super user created successfully {:#?}.", id);
                id
            }

            Err(e) => {
                tracing::error!("Failed to insert user into DB: {:#?}.", e);
                uuid::Uuid::new_v4()
            }
        };
    }
}
```

We `impl` the `Store` struct so we can use its `connection` attribute to talk to the database directly via the `SQLx` crate. The methods peculiar to user management are the `get_user_by_id`, `get_user_by_email`, `activate_user`, and `create_super_user_in_db` for now. They all mean what their names imply. The last one is an administrative method that creates a user with "superpowers". It will be used in the `build` method later on. All of these methods referenced different data models (typically `struct`s) that we've not defined yet. Let's define them in `src/models/users.rs`:

```rust
// src/models/users.rs
#[derive(serde::Serialize, Debug, sqlx::FromRow)]
pub struct User {
    pub id: uuid::Uuid,
    pub email: String,
    pub password: String,
    pub first_name: String,
    pub last_name: String,
    pub is_active: Option<bool>,
    pub is_staff: Option<bool>,
    pub is_superuser: Option<bool>,
    pub thumbnail: Option<String>,
    pub date_joined: time::OffsetDateTime,
}

#[derive(serde::Serialize, Debug, sqlx::FromRow)]
pub struct UserVisible {
    pub id: uuid::Uuid,
    pub email: String,
    pub first_name: String,
    pub last_name: String,
    pub is_active: Option<bool>,
    pub is_staff: Option<bool>,
    pub is_superuser: Option<bool>,
    pub thumbnail: Option<String>,
    pub date_joined: time::OffsetDateTime,
}

#[derive(serde::Serialize)]
pub struct LoggedInUser {
    pub id: uuid::Uuid,
    pub email: String,
    pub password: String,
    pub is_staff: bool,
    pub is_superuser: bool,
}
#[derive(serde::Deserialize, Debug)]
pub struct NewUser {
    pub email: String,
    pub password: String,
    pub first_name: String,
    pub last_name: String,
}

#[derive(serde::Deserialize, Debug)]
pub struct LoginUser {
    pub email: String,
    pub password: String,
}

#[derive(serde::Deserialize, Debug)]
pub struct ActivateUser {
    pub id: uuid::Uuid,
    pub token: String,
}
```

They are just `struct`s that derived from (or implemented) `serde`'s `Serialize` and/or `Deserialize`. Those that derived `Deserialize` are used for incoming requests while those that derived `Serialize` are going to be used as requests' responses. Most of them derived `Debug` too so that they can be logged. Two implemented `sqlx::FromRow`. This is to allow passing them in `sqlx::query_as`. It is a requirement. For this to work, ensure that the data returned by the SQL statement have the same name as the attributes in the struct.

Having paved the way, let's write the login handler:

```rust
// src/routes/users/login.rs
use crate::models::LoginUser;
use crate::startup::AppState;
use crate::utils::verify_password;
use crate::utils::SuccessResponse;
use crate::utils::{CustomAppError, CustomAppJson, ErrorContext};
use axum::{extract::State, http::StatusCode, response::IntoResponse};
use axum_extra::extract::cookie::{Cookie, PrivateCookieJar, SameSite};
use time::Duration;

#[axum::debug_handler]
#[tracing::instrument(name = "login_user", skip(cookies, state, login))]
pub async fn login_user(
    cookies: PrivateCookieJar,
    State(state): State<AppState>,
    CustomAppJson(login): CustomAppJson<LoginUser>,
) -> Result<(PrivateCookieJar, impl IntoResponse), CustomAppError> {
    // Get user from db by email
    let user = state
        .db_store
        .get_user_by_email(&login.email)
        .await
        .map_err(|_| {
            CustomAppError::from((
                "Invalid email or password".to_string(),
                ErrorContext::BadRequest,
            ))
        })?;

    // Verify password
    tokio::task::spawn_blocking(move || {
        verify_password(&user.password, &login.password.as_bytes())
    })
    .await
    .map_err(|_| {
        CustomAppError::from((
            "Server error occurred".to_string(),
            ErrorContext::InternalServerError,
        ))
    })?
    .map_err(|_| {
        CustomAppError::from((
            "Invalid email or password".to_string(),
            ErrorContext::BadRequest,
        ))
    })?;

    // Generate a truly random session id for the user
    let session_id = uuid::Uuid::new_v4().to_string();

    // Save session id in redis
    let mut redis_con = state.redis_store.get().await.map_err(|_| {
        CustomAppError::from((
            "Failed to connect to session store".to_string(),
            ErrorContext::InternalServerError,
        ))
    })?;

    let settings = crate::settings::get_settings().map_err(|_| {
        CustomAppError::from((
            "Failed to read settings".to_string(),
            ErrorContext::InternalServerError,
        ))
    })?;
    let cookie_expiration = settings.secret.cookie_expiration;

    bb8_redis::redis::cmd("SET")
        .arg(session_id.clone())
        .arg(user.id.to_string())
        .arg("EX")
        .arg(cookie_expiration * 60)
        .query_async::<_, String>(&mut *redis_con)
        .await
        .map_err(|_| {
            CustomAppError::from((
                "Failed to save session".to_string(),
                ErrorContext::InternalServerError,
            ))
        })?;

    // Create cookie
    let cookie = Cookie::build(("sessionid", session_id))
        .secure(true)
        .same_site(SameSite::Strict)
        .http_only(true)
        .path("/")
        .max_age(Duration::minutes(cookie_expiration));

    Ok((
        cookies.add(cookie),
        SuccessResponse {
            message: "The authentication process was successful.".to_string(),
            status_code: StatusCode::OK.as_u16(),
        }
        .into_response(),
    ))
}
```

Though long due to error handling, the concept is simple. The handler takes the `PrivateCookieJar` extractor (to extract and help propagate user cookies), `AppState` extractor (to help hold the `AppState` data for the handler to use), and the `CustomAppJson` extractor (to extract the request body). The last argument must be positioned because it consumes the request body. If not, the code will not compile! It returns a `Result` of the tuple (`PrivateCookieJar`, `impl IntoResponse`) or `CustomAppError`. For `PrivateCookieJar` to work, its "value must be returned from the handler as part of the response for the changes to be propagated". In the function body, we first tried to retrieve the requesting user from the database via the email address. Failure to get the user leads to an error being returned. This is where our efforts so far start to shine. Otherwise, we proceeded to verify the user's password. Verification of hashed passwords is CPU-intensive and can block the async runtime. To mitigate this, we spawned a tokio task so that the operation wouldn't be blocked. Next, every user should have a truly random and unique session identification. `uuid` came to the rescue! Since we need to store this session somewhere for subsequent validations as long as the session lives, we chose to store it in redis. Another option is to store it in the PostgreSQL database but this will be slower. A simple key/value store like redis is perfect! The normal Rust's [redis crate][29] doesn't support pooling which is important for a system that will serve a lot of traffic. This made me opt for [bb8-redis][30] which provides `async` and tokio-based redis connection pool. We will set it later but for now, we are already using it to store the `session_id` as the key and `user_id` as the value. After that, we built a cookie which encrypts the generated `session_id`. The cookie is made secure and imposes strict same-site attributes. Of course, we made the cookie HttpOnly to prevent client-side scripts from accessing its embedded data. We used a global path for it and using a configurable expiration period, we set the cookie's `max_age`. As previously stated, it is a requirement to return the cookie as part of the HTTP response.

Next up, let's route this handler:

```rust
// src/routes/users/mod.rs
use axum::{routing::post, Router};

mod login;

pub fn users_routes() -> Router<crate::startup::AppState> {
    Router::new()
        .route("/login", post(login::login_user))
}
```

`users_routes` will build all routes related to user management. Since the route uses the `AppState`, the router returned must specify it. Ensure you make `users_routes` available by exporting it in the main `routes/mod.rs`.

To conclude this long journey, let's set up [bb8-redis][30] connection and include `users_routes` to the main route instance!

```rust
// src/startup.rs
...

#[derive(Clone)]
pub struct AppState {
    ...
    pub redis_store: bb8_redis::bb8::Pool<bb8_redis::RedisConnectionManager>,
}

...

impl Application {
    ...
    pub async fn build(
        settings: crate::settings::Settings,
        test_pool: Option<sqlx::postgres::PgPool>,
    ) -> Result<Self, std::io::Error> {
        ...

        sqlx::migrate!()
            .run(&store.clone().connection)
            .await
            .expect("Failed to migrate");

        // Create superuser if not exists
        store.create_super_user_in_db(&settings).await;
        ...
    }
}

async fn run(
    listener: tokio::net::TcpListener,
    store: crate::store::Store,
    settings: crate::settings::Settings,
) {
    let redis_url = std::env::var("REDIS_URL").expect("Failed to get REDIS_URL.");
    let manager =
        bb8_redis::RedisConnectionManager::new(redis_url).expect("Failed to create redis manager");
    let redis_pool = bb8_redis::bb8::Pool::builder()
        .max_size(15)
        .build(manager)
        .await
        .expect("Failed to create redis pool.");

    ...
    let app_state = AppState {
        ...
        redis_store: redis_pool,
    };
    // build our application with a route
    let app = axum::Router::new()
        ...
        .nest("/api/users", routes::users_routes(app_state.clone()))
        ...;

    ...
}
...
```

Since we want to automatically create the user with "superpowers", we called the utility method for doing that in the `build` method. In the `run` function, we retrieved our machine's redis instance's URL from the `.env` file and created a new bb8 `RedisConnectionManager` from it. From the connection manager, we built a 15-connection pool and added it to the application state. A nice improvement is to make the number of connection pools configurable. Lastly, we added our `users_routes` to the main route using the `nest` method. It helps make our routing composable!

Let's stop here for this part. We'll continue in the next article!

[20]: https://dev.to/sirneij/cryptoflow-building-a-secure-and-scalable-system-with-axum-and-sveltekit-part-0-mn5 "CryptoFlow: Building a secure and scalable system with Axum and SvelteKit - Part 0"
[21]: https://crates.io/crates/axum-extra "Extra utilities for axum"
[22]: https://crates.io/crates/tower-http "Tower middleware and utilities for HTTP clients and servers"
[23]: https://docs.rs/axum-extra/latest/axum_extra/extract/cookie/struct.PrivateCookieJar.html "Available on crate features cookie-private and cookie only."
[24]: https://en.wikipedia.org/wiki/Argon2 "Argon2"
[25]: https://docs.rs/argon2/latest/argon2/ "Crate argon2"
[26]: https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html "Password Storage Cheat Sheet"
[27]: https://github.com/tokio-rs/axum/tree/main/examples/error-handling "Axum error-handling example"
[28]: https://github.com/Sirneij/cryptoflow/blob/main/backend/src/utils/errors.rs "CryptoFlow error handling"
[29]: https://crates.io/crates/redis "Redis driver for Rust."
[30]: https://crates.io/crates/bb8-redis "Full-featured async (tokio-based) redis connection pool (like r2d2)"

# CryptoFlow: Building a secure and scalable system with Axum and SvelteKit - Part 2

## Introduction

[Part 1][40] saw us implement the login process and many other utility functions and setups with which the implementation was made seamless. In this part, we will build on that to allow comprehensive user management as promised. We'll write some middleware to help check whether or not a request is authenticated as well as enable user registration, email verification via token (a 6-digit cryptographically random token will be used), and user logout.

## Source code

The source code for this series is hosted on GitHub via:

[github.com/Sirneij/cryptoflow][100]

## Implementation

### Step 1: User session extraction via middleware

In almost any reasonable system, some services shouldn't be available to anonymous users. These kinds of services are protected by some sort of authentication and authorization mechanism. In our system, we already set up a mechanism to authenticate users. However, we haven't figured out a way to intercept every request coming into our system and check whether or not they should be allowed in. That's what we'll do here! We want to extract certain data from a request and make the decision to reject or accept such a request depending on the data extracted from it. To achieve this, we will roll out a middleware:

```rust
// backend/src/utils/middleware.rs
use crate::startup::AppState;
use crate::utils::get_user_id_from_session;
use axum::{extract::Request, middleware::Next};
use axum::{
    extract::State,
    response::{IntoResponse, Response},
};
use axum_extra::extract::PrivateCookieJar;

#[tracing::instrument(
    name = "validate_authentication_session",
    skip(cookies, state, req, next)
)]
pub async fn validate_authentication_session(
    cookies: PrivateCookieJar,
    State(state): State<AppState>,
    req: Request,
    next: Next,
) -> Result<impl IntoResponse, Response> {
    // Use the utility function to get the user ID from the session
    match get_user_id_from_session(&cookies, &state.redis_store, false).await {
        Ok(_user_id) => Ok(next.run(req).await),
        Err(error) => Err(error.into_response()),
    }
}
```

One major selling point of [axum][41] is its ease of writing middleware. Just look at what we have up there! It's just a beauty!!! Though some things have been abstracted away (especially retrieving the cookie and processing it), it is still unmatched! That said, writing middleware in axum [can take different shapes][42], we decided to stick to using the `axum::middleware::from_fn` (`axum::middleware::from_fn_with_state` precisely) because our middleware is just for this system, built with axum, to extract request cookies and make decisions. In axum, a function regarded as middleware should take the `Request` and `Next` types (in v0.6, these types were required to be bounded by `<B>` generics) alongside other arguments and return something that implements `IntoResponse`. If all requirements are met, returning `next.run(req).await` allows the request to proceed to the main service it wanted.

An integral part of the middleware above is the `get_user_id_from_session` and it has this definition:

```rust
// backend/src/utils/user.rs
use crate::utils::{CustomAppError, ErrorContext};
use axum_extra::extract::PrivateCookieJar;
use bb8_redis::bb8;
use uuid::Uuid;

#[tracing::instrument(
    name = "get_user_id_from_session",
    skip(cookies, redis_store, is_logout)
)]
pub async fn get_user_id_from_session(
    cookies: &PrivateCookieJar,
    redis_store: &bb8::Pool<bb8_redis::RedisConnectionManager>,
    is_logout: bool,
) -> Result<(Uuid, String), CustomAppError> {
    let session_id = cookies
        .get("sessionid")
        .map(|cookie| cookie.value().to_owned())
        .ok_or_else(|| {
            CustomAppError::from((
                "Session ID not found because you are not authenticated".to_string(),
                ErrorContext::UnauthorizedAccess,
            ))
        })?;

    let mut redis_con = redis_store.get().await.map_err(|_| {
        CustomAppError::from((
            "Failed to get redis connection".to_string(),
            ErrorContext::InternalServerError,
        ))
    })?;

    tracing::debug!("Session ID: {}", session_id);

    let user_id: String = bb8_redis::redis::cmd("GET")
        .arg(&session_id)
        .query_async(&mut *redis_con)
        .await
        .map_err(|_| {
            CustomAppError::from((
                "You are not authorized since you don't seem to have been authenticated"
                    .to_string(),
                ErrorContext::UnauthorizedAccess,
            ))
        })?;

    let user_uuid = Uuid::parse_str(&user_id).map_err(|_| {
        CustomAppError::from((
            "Invalid user ID format".to_string(),
            ErrorContext::InternalServerError,
        ))
    })?;

    if is_logout {
        bb8_redis::redis::cmd("DEL")
            .arg(&session_id)
            .query_async::<_, i64>(&mut *redis_con)
            .await
            .map_err(|_| {
                CustomAppError::from((
                    "Failed to delete session ID from redis".to_string(),
                    ErrorContext::InternalServerError,
                ))
            })?;
    }

    Ok((user_uuid, session_id))
}
```

The function first tries retrieving the `session_id` stored in the cookies (as done in the `login_user` in [part 1][40]). If successful, it then goes to do the same from the redis. This is the second layer of security. In case the `session_id` isn't found in either, appropriate errors are returned. The function takes a flag, `is_logout`, which determines whether or not the `session_id` will be deleted from the redis (which is the case for the logout operation).

With that concluded, let's use it. But before then, let's write the logout handler.

### Step 2: Logging users out

We'll create a new file, `backend/src/routes/users/logout.rs`, and fill it with this:

```rust
use crate::startup::AppState;
use crate::utils::CustomAppError;
use crate::utils::SuccessResponse;
use axum::{extract::State, http::StatusCode, response::IntoResponse};
use axum_extra::extract::cookie::{Cookie, PrivateCookieJar};

#[axum::debug_handler]
#[tracing::instrument(name = "logout_user", skip(cookies, state))]
pub async fn logout_user(
    cookies: PrivateCookieJar,
    State(state): State<AppState>,
) -> Result<(PrivateCookieJar, impl IntoResponse), CustomAppError> {
    // Get user_id and session_id from cookie and delete it
    let (_, _) = crate::utils::get_user_id_from_session(&cookies, &state.redis_store, true).await?;

    Ok((
        cookies.remove(Cookie::from("sessionid")),
        SuccessResponse {
            message: "The unauthentication process was successful.".to_string(),
            status_code: StatusCode::OK.as_u16(),
        }
        .into_response(),
    ))
}
```

It is very simple. We utilized the `get_user_id_from_session` utility function, passing `is_logout=true` to delete the session from redis in case everything goes as planned. Remember that to propagate any action on `PrivateCookieJar`, it must be returned with your response.

Next, let's fix it up with our routes:

```rust
// src/routes/users/mod.rs
...
mod logout

pub fn users_routes(state: crate::startup::AppState) -> Router<crate::startup::AppState> {
    Router::new()
        .route("/logout", post(logout::logout_user))
        .route_layer(axum::middleware::from_fn_with_state(
            state.clone(),
            validate_authentication_session,
        ))
        ...
}
```

`users_routes` now needs to take `AppState` as an argument (you need to pass this in `backend/src/startup.rs` accordingly). This is because `from_fn_with_state` needs an instance of it to work. Our middleware was also added up. This ensures that only authenticated users can access `/api/users/logout` route. As demonstrated, all routes before the application of a middleware have that middleware applied to them while those after are free from the middleware's requirements.

You can now test the login/logout process and everything should be great.

### Step 3: Registering users

Before users can log in (not to mention logout), they need a (verified) account. It's time to start the process of getting such an account:

```rust
// backend/src/routes/users/register.rs
use crate::models::NewUser;
use crate::startup::AppState;
use crate::utils::SuccessResponse;
use crate::utils::{CustomAppError, CustomAppJson, ErrorContext};
use argon2::password_hash::rand_core::{OsRng, RngCore};
use axum::{extract::State, http::StatusCode, response::IntoResponse};
use sha2::{Digest, Sha256};

#[axum::debug_handler]
#[tracing::instrument(name = "register_user", skip(state, new_user),fields(user_email = new_user.email, user_first_name = new_user.first_name, user_last_name = new_user.last_name))]
pub async fn register_user(
    State(state): State<AppState>,
    CustomAppJson(new_user): CustomAppJson<NewUser>,
) -> Result<impl IntoResponse, CustomAppError> {
    let hashed_password = crate::utils::hash_password(&new_user.password.as_bytes()).await;

    let user = state
        .db_store
        .create_user(
            &new_user.first_name,
            &new_user.last_name,
            &new_user.email,
            &hashed_password,
        )
        .await?;

    // Generate a truly random activation code for the user using argon2::password_hash::rand_core::OsRng
    let activation_code = (OsRng.next_u32() % 900000 + 100000).to_string();
    // Hash the activation code
    let mut hasher = Sha256::new();
    hasher.update(activation_code.as_bytes());
    let hashed_activation_code = format!("{:x}", hasher.finalize());

    // Save activation code in redis
    let mut redis_con = state.redis_store.get().await.map_err(|_| {
        CustomAppError::from((
            "Failed to get redis connection".to_string(),
            ErrorContext::InternalServerError,
        ))
    })?;

    let settings = crate::settings::get_settings().map_err(|_| {
        CustomAppError::from((
            "Failed to read settings".to_string(),
            ErrorContext::InternalServerError,
        ))
    })?;
    let activation_code_expiration_in_seconds = settings.secret.token_expiration * 60;

    bb8_redis::redis::cmd("SET")
        .arg(user.id.to_string())
        .arg(hashed_activation_code)
        .arg("EX")
        .arg(activation_code_expiration_in_seconds)
        .query_async::<_, String>(&mut *redis_con)
        .await
        .map_err(|_| {
            CustomAppError::from((
                "Failed to save activation code".to_string(),
                ErrorContext::InternalServerError,
            ))
        })?;

    // Send activation code to user's email
    crate::utils::send_multipart_email(
        "Welcome to CryptoFlow with Rust (axum) and SvelteKit".to_string(),
        user,
        state.clone(),
        "user_welcome.html",
        activation_code,
    )
    .await
    .map_err(|_| {
        CustomAppError::from((
            "Failed to send activation email".to_string(),
            ErrorContext::InternalServerError,
        ))
    })?;

    Ok(SuccessResponse {
        message: "Registration complete! Check your email for a verification code to activate your account.".to_string(),
        status_code: StatusCode::CREATED.as_u16(),
    }.into_response())
}
```

As usual, error handling took a bunch of space but it's okay! We just started by hashing the provided password and went straight to create the user in our database using the `create_user` method on `db_store` (was written in [part 1][40]). This creation sets the `is_active` to `false` pending when the user's email is confirmed. Then, a 6-digit cryptographically random string is generated and its sha256 hash (we don't want to save the plain token) is saved temporarily (for `token_expiration * 60` seconds, `15 * 60` by default) in redis (the reason for sending a token instead of sending a verification link was explained [here][45]). The token is then sent to the email used in registration. Let's take a look at the email-sending utility:

```rust
// backend/src/utils/email.rs
use lettre::AsyncTransport;

#[tracing::instrument(
    name = "Generic e-mail sending function.",
    skip(
        subject,
        html_content,
        text_content
    ),
    fields(
        recipient_email = %user.email,
        recipient_first_name = %user.first_name,
        recipient_last_name = %user.last_name
    )
)]
pub async fn send_email(
    user: crate::models::UserVisible,
    subject: impl Into<String>,
    html_content: impl Into<String>,
    text_content: impl Into<String>,
) -> Result<(), String> {
    let settings = crate::settings::get_settings().expect("Failed to read settings.");

    let email = lettre::Message::builder()
        .from(
            format!(
                "{} <{}>",
                "CryptoFlow with axum and SvelteKit",
                settings.email.host_user.clone()
            )
            .parse()
            .map_err(|e| {
                tracing::error!("Could not parse 'from' email address: {:#?}", e);
                format!("Could not parse 'from' email address: {:#?}", e)
            })?,
        )
        .to(format!(
            "{} <{}>",
            [user.first_name, user.last_name].join(" "),
            user.email
        )
        .parse()
        .map_err(|e| {
            tracing::error!("Could not parse 'to' email address: {:#?}", e);
            format!("Could not parse 'to' email address: {:#?}", e)
        })?)
        .subject(subject)
        .multipart(
            lettre::message::MultiPart::alternative()
                .singlepart(
                    lettre::message::SinglePart::builder()
                        .header(lettre::message::header::ContentType::TEXT_PLAIN)
                        .body(text_content.into()),
                )
                .singlepart(
                    lettre::message::SinglePart::builder()
                        .header(lettre::message::header::ContentType::TEXT_HTML)
                        .body(html_content.into()),
                ),
        )
        .unwrap();

    let creds = lettre::transport::smtp::authentication::Credentials::new(
        settings.email.host_user,
        settings.email.host_user_password,
    );

    // Open a remote connection to gmail
    let mailer: lettre::AsyncSmtpTransport<lettre::Tokio1Executor> =
        lettre::AsyncSmtpTransport::<lettre::Tokio1Executor>::relay(&settings.email.host)
            .unwrap()
            .credentials(creds)
            .build();

    // Send the email
    match mailer.send(email).await {
        Ok(_) => {
            tracing::info!("Email sent successfully.");
            Ok(())
        }
        Err(e) => {
            tracing::error!("Could not send email: {:#?}", e);
            Err(format!("Could not send email: {:#?}", e))
        }
    }
}

#[tracing::instrument(
    name = "Generic multipart e-mail sending function.",
    skip(
        user,
        state,
        template_name
    ),
    fields(
        recipient_user_id = %user.id,
        recipient_email = %user.email,
        recipient_first_name = %user.first_name,
        recipient_last_name = %user.last_name
    )
)]
pub async fn send_multipart_email(
    subject: String,
    user: crate::models::UserVisible,
    state: crate::startup::AppState,
    template_name: &str,
    issued_token: String,
) -> Result<(), String> {
    let settings = crate::settings::get_settings().expect("Unable to load settings.");
    let title = subject.clone();

    let now = chrono::Local::now();
    let expiration_time = now + chrono::Duration::minutes(settings.secret.token_expiration);
    let exact_time = expiration_time.format("%A %B %d, %Y at %r").to_string();

    let template = state.env.get_template(template_name).unwrap();
    let ctx = minijinja::context! {
        title => &title,
        user_id => &user.id,
        domain => &settings.frontend_url,
        token => &issued_token,
        expiration_time => &settings.secret.token_expiration,
        exact_time => &exact_time,
    };
    let html_text = template.render(ctx).unwrap();

    let text = format!(
        r#"
        Thanks for signing up for a CryptoFlow with Rust (axum) and SvelteKit. We're excited to have you on board!

        For future reference, your user ID number is {}.

        Please visit {}/auth/activate/{} and input the token below to activate your account:

        {}


        Please note that this is a one-time use token and it will expire in {} minutes ({}).


        Thanks,

        CryptoFlow with Rust (axum) and SvelteKit Team
        "#,
        user.id,
        settings.frontend_url,
        user.id,
        issued_token,
        settings.secret.token_expiration,
        exact_time
    );

    tokio::spawn(send_email(user, subject, html_text, text));
    Ok(())
}
```

Briefly (a detailed explanation can be found [here][43]), it uses the `lettre` crate to build and send emails. The emails support HTML and have plaintext fallback in case the email server doesn't support HTML. For templating, we used `minijinja` which we will set up next!

```rust
// backend/src/startup.rs
...
#[derive(Clone)]
pub struct AppState {
    ...
    pub env: minijinja::Environment<'static>,
    ...
}
...

async fn run(
    listener: tokio::net::TcpListener,
    store: crate::store::Store,
    settings: crate::settings::Settings,
) {
    ...
    let mut env = minijinja::Environment::new();
    env.set_loader(minijinja::path_loader("templates"));
    let app_state = AppState {
        ...
        env,
        ...
    };
    ...
}
...
```

Kindly create the `templates` folder at the root of the `backend` folder. In it, put this HTML file:

```html
<!--backend/templates/user_welcome.html-->
<!DOCTYPE html>
<html>
  <head>
    <meta name="viewport" content="width=device-width" />
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <title>{{ title }}</title>
  </head>
  <body>
    <table style="background: #ffffff; border-radius: 1rem; padding: 30px 0px">
      <tbody>
        <tr>
          <td style="padding: 0px 30px">
            <h3 style="margin-bottom: 0px; color: #000000">Hello,</h3>
            <p>
              Thanks for signing up for a CryptoFlow with Rust (axum) and
              SvelteKit. We're excited to have you on board!
            </p>
          </td>
        </tr>
        <tr>
          <td style="padding: 0px 30px">
            <p>For future reference, your user ID is #{{ user_id }}.</p>
            <p>
              Please visit
              <a href="{{ domain }}/auth/activate/{{ user_id }}">
                {{ domain }}/auth/activate/{{ user_id }}
              </a>
              and input the OTP below to activate your account:
            </p>
          </td>
        </tr>

        <tr>
          <td style="padding: 10px 30px; text-align: center">
            <strong style="display: block; color: #00a856">
              One Time Password (OTP)
            </strong>
            <table style="margin: 10px 0px" width="100%">
              <tbody>
                <tr>
                  <td
                    style="
                      padding: 25px;
                      background: #faf9f5;
                      border-radius: 1rem;
                    "
                  >
                    <strong
                      style="
                        letter-spacing: 8px;
                        font-size: 24px;
                        color: #000000;
                      "
                    >
                      {{ token }}
                    </strong>
                  </td>
                </tr>
              </tbody>
            </table>
            <small style="display: block; color: #6c757d; line-height: 19px">
              <strong>
                Please note that this is a one-time use token and it will expire
                in {{ expiration_time }} minutes ({{ exact_time }}).
              </strong>
            </small>
          </td>
        </tr>

        <tr>
          <td style="padding: 0px 30px">
            <hr style="margin: 0" />
          </td>
        </tr>
        <tr>
          <td style="padding: 30px 30px">
            <table>
              <tbody>
                <tr>
                  <td>
                    <strong>
                      Kind Regards,<br />
                      CryptoFlow with Rust (axum) and SvelteKit Team
                    </strong>
                  </td>
                  <td></td>
                </tr>
              </tbody>
            </table>
          </td>
        </tr>
      </tbody>
    </table>
  </body>
</html>
```

It's a simple but nice-looking email template built in this [series][44].

To wrap up, let's write the user activation or token verification handler.

### Step 4: User activation and token verification

Currently, the registration process isn't complete yet as the registered users cannot log in to our system. Their email addresses must be verified before such can be allowed. Here is the function that handles the verification:

```rust
// backend/src/routes/users/activate_account.rs
use crate::{
    models::ActivateUser,
    startup::AppState,
    utils::{CustomAppError, CustomAppJson, ErrorContext, SuccessResponse},
};
use axum::{extract::State, http::StatusCode, response::IntoResponse};
use sha2::{Digest, Sha256};

#[axum::debug_handler]
#[tracing::instrument(name = "activate_user_account", skip(state, acc_user))]
pub async fn activate_user_account(
    State(state): State<AppState>,
    CustomAppJson(acc_user): CustomAppJson<ActivateUser>,
) -> Result<impl IntoResponse, CustomAppError> {
    let mut redis_con = state.redis_store.get().await.map_err(|_| {
        CustomAppError::from((
            "Failed to get redis connection".to_string(),
            ErrorContext::InternalServerError,
        ))
    })?;

    let mut hasher = Sha256::new();
    hasher.update(acc_user.token.as_bytes());
    let hashed_token = format!("{:x}", hasher.finalize());

    let hashed_activation_code: String = bb8_redis::redis::cmd("GET")
        .arg(&acc_user.id.to_string())
        .query_async(&mut *redis_con)
        .await
        .map_err(|_| {
            CustomAppError::from((
                "This activation has been used or expired".to_string(),
                ErrorContext::BadRequest,
            ))
        })?;

    if hashed_activation_code == hashed_token {
        state.db_store.activate_user(&acc_user.id).await?;

        // Delete activation code from redis
        bb8_redis::redis::cmd("DEL")
            .arg(&acc_user.id.to_string())
            .query_async::<_, i64>(&mut *redis_con)
            .await
            .map_err(|_| {
                CustomAppError::from((
                    "Failed to delete activation code from Redis".to_string(),
                    ErrorContext::InternalServerError,
                ))
            })?;

        Ok(SuccessResponse {
            message: "The activation process was successful.".to_string(),
            status_code: StatusCode::OK.as_u16(),
        }
        .into_response())
    } else {
        return Err(CustomAppError::from((
            "Activation code not found or expired".to_string(),
            ErrorContext::BadRequest,
        )));
    }
}
```

It's simple. We require that the user provides the token sent. We then find its hash and compare it to what we have in redis for that user. If they are the same, the user gets activated. Otherwise, we send an error. Though not covered in this series, a nice feature to have is allowing users to regenerate tokens so that they won't be locked out of our system forever (we want users!!!).

> **NOTE: I also implemented a handler that retrieves the currently logged-in user. It's simple and can be seen [here][46].**

The entire `user_routes` should now look like this:

```rust
// backend/src/routes/users/mod.rs

use crate::utils::validate_authentication_session;
use axum::{
    routing::{get, post},
    Router,
};

mod activate_account;
mod current_user;
mod login;
mod logout;
mod register;

pub fn users_routes(state: crate::startup::AppState) -> Router<crate::startup::AppState> {
    Router::new()
        .route("/logout", post(logout::logout_user))
        .route("/current", get(current_user::get_current_user))
        .route_layer(axum::middleware::from_fn_with_state(
            state.clone(),
            validate_authentication_session,
        ))
        .route("/login", post(login::login_user))
        .route("/register", post(register::register_user))
        .route("/activate", post(activate_account::activate_user_account))
}
```

With that, we are done with user management stuff. It's not feature-complete though (I gave suggestions). Let's move on to the Q&A service in the next few articles. See ya!!!

[40]: https://dev.to/sirneij/cryptoflow-building-a-secure-and-scalable-system-with-axum-and-sveltekit-part-1-2mnn "CryptoFlow: Building a secure and scalable system with Axum and SvelteKit - Part 1"
[41]: https://docs.rs/axum/latest/axum/ "Axum is a web application framework that focuses on ergonomics and modularity."
[42]: https://docs.rs/axum/latest/axum/middleware/index.html#writing-middleware "Writing middleware"
[43]: https://dev.to/sirneij/authentication-system-using-rust-actix-web-and-sveltekit-user-registration-580h "Authentication system using rust (actix-web) and sveltekit - User Registration"
[44]: https://dev.to/sirneij/series/23239 "Secure and performant full-stack authentication system using Golang and SvelteKit Series"
[45]: https://security.stackexchange.com/questions/213944/account-verification-emails-with-links-vs-codes "Account verification emails with links vs codes"
[46]: https://github.com/Sirneij/cryptoflow/blob/main/backend/src/routes/users/current_user.rs "Current user"

# CryptoFlow: Building a secure and scalable system with Axum and SvelteKit - Part 3

## Introduction

Having built out the system's user management arm, it is time to delve into providing Q&A services for our users. We want the users of our system to ask questions and provide answers (both should support regular and some advanced markdown commands). Questions and answers should also be easily managed (they should be retrievable, deletable and updatable). Before all that, we need to write some utility functions and methods.

## Source code

The source code for this series is hosted on GitHub via:

[github.com/Sirneij/cryptoflow][100]

## Implementation

### Step 1: Write Q&A structs and utility functions

To start, we need to set the pace for writing the features our system implements. We'll incept with the models:

```rust
// backend/src/models/qa.rs
use serde_json::Value as JsonValue;
use sqlx::FromRow;
use uuid::Uuid;

#[derive(Debug, serde::Serialize, Clone)]
pub struct Question {
    pub id: Uuid,
    pub title: String,
    pub slug: String,
    pub content: String,
    pub raw_content: String,
    pub author: Uuid,
    pub created_at: chrono::DateTime<chrono::Utc>,
    pub updated_at: chrono::DateTime<chrono::Utc>,
}

#[derive(serde::Deserialize, Debug)]
pub struct NewQuestion {
    pub title: String,
    pub content: String,
    pub tags: String,
}

#[derive(serde::Serialize, Debug)]
pub struct CreateQuestion {
    pub title: String,
    pub slug: String,
    pub content: String,
    pub raw_content: String,
    pub author: Uuid,
    pub tags: Vec<String>,
}

#[derive(serde::Deserialize, Debug)]
pub struct UpdateQuestion {
    pub title: String,
    pub tags: String,
    pub content: String,
}

#[derive(serde::Serialize, Debug)]
pub struct Answer {
    pub id: Uuid,
    pub content: String,
    pub raw_content: String,
    pub author: Uuid,
    pub created_at: chrono::DateTime<chrono::Utc>,
    pub updated_at: chrono::DateTime<chrono::Utc>,
}

#[derive(serde::Deserialize, Debug)]
pub struct NewAnswer {
    pub content: String,
}

#[derive(serde::Serialize, Debug)]
pub struct CreateAnswer {
    pub content: String,
    pub raw_content: String,
    pub author: Uuid,
    pub question: Uuid,
}

#[derive(serde::Serialize, Debug)]
pub struct UpdateAnswer {
    pub content: String,
    pub raw_content: String,
    pub author: Uuid,
    pub answer_id: Uuid,
}

#[derive(serde::Serialize, serde::Deserialize, Debug, FromRow)]
pub struct Tag {
    pub id: String,
    pub name: String,
    pub symbol: String,
}

#[derive(serde::Serialize, Debug)]
pub struct QuestionAuthorWithTags {
    pub id: Uuid,
    pub title: String,
    pub slug: String,
    pub content: String,
    pub raw_content: String,
    pub author: crate::models::UserVisible,
    pub created_at: chrono::DateTime<chrono::Utc>,
    pub updated_at: chrono::DateTime<chrono::Utc>,
    pub tags: Vec<Tag>,
}

#[derive(serde::Serialize, Debug)]
pub struct AnswerAuthor {
    pub id: Uuid,
    pub content: String,
    pub raw_content: String,
    pub author: crate::models::UserVisible,
    pub created_at: chrono::DateTime<chrono::Utc>,
    pub updated_at: chrono::DateTime<chrono::Utc>,
}

#[derive(FromRow, Debug)]
pub struct QuestionAuthorWithTagsQueryResult {
    // Fields from `questions`
    pub id: Uuid,
    pub title: String,
    pub slug: String,
    pub content: String,
    pub raw_content: String,
    pub created_at: chrono::DateTime<chrono::Utc>,
    pub updated_at: chrono::DateTime<chrono::Utc>,
    // JSON aggregation of tags
    pub tags_json: JsonValue,
    // Fields from `users`
    pub user_id: Uuid,
    pub user_email: String,
    pub user_first_name: String,
    pub user_last_name: String,
    pub user_is_active: Option<bool>,
    pub user_is_staff: Option<bool>,
    pub user_is_superuser: Option<bool>,
    pub user_thumbnail: Option<String>,
    pub user_date_joined: chrono::DateTime<chrono::Utc>,
}

#[derive(FromRow, Debug)]
pub struct AnswerAuthorQueryResult {
    // Fields from `answers`
    pub id: Uuid,
    pub content: String,
    pub raw_content: String,
    pub created_at: chrono::DateTime<chrono::Utc>,
    pub updated_at: chrono::DateTime<chrono::Utc>,
    // Fields from `users`
    pub user_id: Uuid,
    pub user_email: String,
    pub user_first_name: String,
    pub user_last_name: String,
    pub user_is_active: Option<bool>,
    pub user_is_staff: Option<bool>,
    pub user_is_superuser: Option<bool>,
    pub user_thumbnail: Option<String>,
    pub user_date_joined: chrono::DateTime<chrono::Utc>,
}

impl From<AnswerAuthorQueryResult> for AnswerAuthor {
    fn from(query_result: AnswerAuthorQueryResult) -> Self {
        AnswerAuthor {
            id: query_result.id,
            content: query_result.content,
            raw_content: query_result.raw_content,
            created_at: query_result.created_at,
            updated_at: query_result.updated_at,
            author: crate::models::UserVisible {
                id: query_result.user_id,
                email: query_result.user_email,
                first_name: query_result.user_first_name,
                last_name: query_result.user_last_name,
                is_active: query_result.user_is_active,
                is_staff: query_result.user_is_staff,
                is_superuser: query_result.user_is_superuser,
                thumbnail: query_result.user_thumbnail,
                date_joined: query_result.user_date_joined,
            },
        }
    }
}
```

They're just some simple structs that model the `questions` and `answers` tables created in [part][50] as well as other operations that will be useful in the handlers. Let's write the store operations next:

```rust
// backend/src/store/question.rs
use sqlx::Row;
use std::collections::HashMap;

impl crate::store::Store {
    #[tracing::instrument(name = "get_question_from_db", skip(question_id))]
    pub async fn get_question_from_db(
        &self,
        transaction: Option<&mut sqlx::Transaction<'_, sqlx::Postgres>>,
        question_id: uuid::Uuid,
    ) -> Result<crate::models::QuestionAuthorWithTags, sqlx::Error> {
        let query = sqlx::query_as::<_, crate::models::QuestionAuthorWithTagsQueryResult>(
            crate::utils::QUESTION_AUTHOR_WITH_TAGS_QUERY,
        )
        .bind(question_id);

        let result = match transaction {
            Some(t) => query.fetch_one(&mut **t).await?,
            None => query.fetch_one(&self.connection).await?,
        };

        let question_author_with_tags = crate::models::QuestionAuthorWithTags {
            id: result.id,
            title: result.title,
            slug: result.slug,
            content: result.content,
            raw_content: result.raw_content,
            author: crate::models::UserVisible {
                id: result.user_id,
                email: result.user_email,
                first_name: result.user_first_name,
                last_name: result.user_last_name,
                is_active: result.user_is_active,
                is_staff: result.user_is_staff,
                is_superuser: result.user_is_superuser,
                thumbnail: result.user_thumbnail,
                date_joined: result.user_date_joined,
            },
            created_at: result.created_at,
            updated_at: result.updated_at,
            tags: serde_json::from_value(result.tags_json)
                .map_err(|e| sqlx::Error::Protocol(e.to_string().into()))?,
        };

        Ok(question_author_with_tags)
    }

    #[tracing::instrument(name = "create_question_in_db", skip(create_question))]
    pub async fn create_question_in_db(
        &self,
        create_question: crate::models::CreateQuestion,
    ) -> Result<crate::models::QuestionAuthorWithTags, sqlx::Error> {
        let mut transaction = self.connection.begin().await?;
        let q_id = match sqlx::query(
            "INSERT INTO questions (title, slug, content, raw_content, author) VALUES ($1, $2, $3, $4, $5) RETURNING id",
        )
        .bind(&create_question.title)
        .bind(&create_question.slug)
        .bind(&create_question.content)
        .bind(&create_question.raw_content)
        .bind(&create_question.author).map(|row: sqlx::postgres::PgRow| -> uuid::Uuid { row.get("id") })
        .fetch_one(&mut *transaction)
        .await {
            Ok(id) => id,
            Err(e) => return Err(e),
        };

        match sqlx::query("INSERT INTO question_tags (question, tag) SELECT $1, * FROM UNNEST($2)")
            .bind(q_id)
            .bind(&create_question.tags)
            .execute(&mut *transaction)
            .await
        {
            Ok(_) => {
                tracing::info!("Tag ids inserted successfully");
            }
            Err(e) => return Err(e),
        }

        let question_author_with_tags = self
            .get_question_from_db(Some(&mut transaction), q_id)
            .await?;

        transaction.commit().await?;

        Ok(question_author_with_tags)
    }

    #[tracing::instrument(name = "update_question_in_db", skip(update_question))]
    pub async fn update_question_in_db(
        &self,
        question_id: uuid::Uuid,
        update_question: crate::models::CreateQuestion,
    ) -> Result<crate::models::QuestionAuthorWithTags, sqlx::Error> {
        let mut transaction = self.connection.begin().await?;

        let q_id = match sqlx::query(
            "UPDATE questions SET title = $1, slug = $2, content = $3, raw_content = $4 WHERE id = $5 AND author = $6 RETURNING id",
        )
        .bind(&update_question.title)
        .bind(&update_question.slug)
        .bind(&update_question.content)
        .bind(&update_question.raw_content)
        .bind(question_id)
        .bind(&update_question.author)
        .map(|row: sqlx::postgres::PgRow| -> uuid::Uuid { row.get("id") })
        .fetch_one(&mut *transaction)
        .await {
            Ok(id) => id,
            Err(e) => return Err(e),
        };

        match sqlx::query("DELETE FROM question_tags WHERE question = $1")
            .bind(q_id)
            .execute(&mut *transaction)
            .await
        {
            Ok(_) => {
                tracing::info!("Tag ids deleted successfully");
            }
            Err(e) => return Err(e),
        }

        match sqlx::query("INSERT INTO question_tags (question, tag) SELECT $1, * FROM UNNEST($2)")
            .bind(q_id)
            .bind(&update_question.tags)
            .execute(&mut *transaction)
            .await
        {
            Ok(_) => {
                tracing::info!("Tag ids inserted successfully");
            }
            Err(e) => return Err(e),
        }

        let question_author_with_tags = self
            .get_question_from_db(Some(&mut transaction), q_id)
            .await?;

        transaction.commit().await?;

        Ok(question_author_with_tags)
    }

    #[tracing::instrument(name = "get_all_questions_from_db")]
    pub async fn get_all_questions_from_db(
        &self,
    ) -> Result<Vec<crate::models::QuestionAuthorWithTags>, sqlx::Error> {
        let results = sqlx::query_as::<_, crate::models::QuestionAuthorWithTagsQueryResult>(
            crate::utils::QUESTION_AUTHOR_WITH_TAGS_QUERY_ALL,
        )
        .fetch_all(&self.connection)
        .await?;

        let mut questions_map: HashMap<uuid::Uuid, crate::models::QuestionAuthorWithTags> =
            HashMap::new();

        for result in results {
            let tags: Vec<crate::models::Tag> = serde_json::from_value(result.tags_json)
                .map_err(|e| sqlx::Error::Protocol(e.to_string().into()))?;

            questions_map
                .entry(result.id)
                .or_insert(crate::models::QuestionAuthorWithTags {
                    id: result.id,
                    title: result.title,
                    slug: result.slug,
                    content: result.content,
                    raw_content: result.raw_content,
                    author: crate::models::UserVisible {
                        id: result.user_id,
                        email: result.user_email,
                        first_name: result.user_first_name,
                        last_name: result.user_last_name,
                        is_active: result.user_is_active,
                        is_staff: result.user_is_staff,
                        is_superuser: result.user_is_superuser,
                        thumbnail: result.user_thumbnail,
                        date_joined: result.user_date_joined,
                    },
                    created_at: result.created_at,
                    updated_at: result.updated_at,
                    tags,
                });
        }

        Ok(questions_map.into_values().collect())
    }

    #[tracing::instrument(name = "delete_question_from_db")]
    pub async fn delete_question_from_db(
        &self,
        author_id: uuid::Uuid,
        question_id: uuid::Uuid,
    ) -> Result<(), sqlx::Error> {
        let deleted =
            sqlx::query("DELETE FROM questions WHERE id = $1 AND author = $2 RETURNING id")
                .bind(question_id)
                .bind(author_id)
                .fetch_optional(&self.connection)
                .await?;

        if deleted.is_none() {
            tracing::warn!(
                "Attempt to delete question with id {} by non-author {}",
                question_id,
                author_id
            );
            return Err(sqlx::Error::RowNotFound);
        }

        Ok(())
    }
}
```

These are basic operations. In `create_question_in_db` and `update_question_in_db`, however, we used the notion of transactions because we have more than one query to execute at a time. To avoid data corruption, we want all of them executed or none to be executed (the basis of atomicity). Transactions guarantee this.

```rust
// backend/src/store/answer.rs
use sqlx::Row;

impl crate::store::Store {
    #[tracing::instrument(name = "get_an_answer_from_db", skip(transaction, answer_id))]
    pub async fn get_an_answer_from_db(
        &self,
        transaction: Option<&mut sqlx::Transaction<'_, sqlx::Postgres>>,
        answer_id: uuid::Uuid,
    ) -> Result<crate::models::AnswerAuthor, sqlx::Error> {
        let query = sqlx::query_as::<_, crate::models::AnswerAuthorQueryResult>(
            crate::utils::ANSWER_AUTHOR_QUERY,
        )
        .bind(answer_id);

        let query_result = match transaction {
            Some(t) => query.fetch_one(&mut **t).await?,
            None => query.fetch_one(&self.connection).await?,
        };

        Ok(query_result.into())
    }

    #[tracing::instrument(name = "create_answer_in_db", skip(create_answer))]
    pub async fn create_answer_in_db(
        &self,
        create_answer: crate::models::CreateAnswer,
    ) -> Result<crate::models::AnswerAuthor, sqlx::Error> {
        let mut transaction = self.connection.begin().await?;
        let a_id = match sqlx::query(
            "INSERT INTO answers (content, raw_content, author, question) VALUES ($1, $2, $3, $4) RETURNING id",
        )
        .bind(&create_answer.content)
        .bind(&create_answer.raw_content)
        .bind(&create_answer.author)
        .bind(&create_answer.question)
        .map(|row: sqlx::postgres::PgRow| -> uuid::Uuid { row.get("id") })
        .fetch_one(&mut *transaction)
        .await {
            Ok(id) => id,
            Err(e) => return Err(e),
        };

        let answer_author = self
            .get_an_answer_from_db(Some(&mut transaction), a_id)
            .await?;

        transaction.commit().await?;

        Ok(answer_author)
    }

    #[tracing::instrument(name = "get_answers_from_db")]
    pub async fn get_answers_from_db(
        &self,
        transaction: Option<&mut sqlx::Transaction<'_, sqlx::Postgres>>,
        question_id: uuid::Uuid,
    ) -> Result<Vec<crate::models::AnswerAuthor>, sqlx::Error> {
        let query = sqlx::query_as::<_, crate::models::AnswerAuthorQueryResult>(
            crate::utils::ANSWER_AUTHOR_QUERY_VIA_QUESTION_ID,
        )
        .bind(question_id);

        let results = match transaction {
            Some(t) => query.fetch_all(&mut **t).await?,
            None => query.fetch_all(&self.connection).await?,
        };

        let answers = results.into_iter().map(|result| result.into()).collect();

        Ok(answers)
    }

    #[tracing::instrument(name = "delete_answer_from_db")]
    pub async fn delete_answer_from_db(
        &self,
        author_id: uuid::Uuid,
        answer_id: uuid::Uuid,
    ) -> Result<(), sqlx::Error> {
        let deleted = sqlx::query("DELETE FROM answers WHERE id = $1 AND author = $2 RETURNING id")
            .bind(answer_id)
            .bind(author_id)
            .fetch_optional(&self.connection)
            .await?;

        if deleted.is_none() {
            tracing::warn!(
                "Attempt to delete question with id {} by non-author {}",
                answer_id,
                author_id
            );
            return Err(sqlx::Error::RowNotFound);
        }

        Ok(())
    }

    #[tracing::instrument(name = "update_answer_in_db", skip(update_answer))]
    pub async fn update_answer_in_db(
        &self,
        update_answer: crate::models::UpdateAnswer,
    ) -> Result<crate::models::AnswerAuthor, sqlx::Error> {
        let a_id = match sqlx::query(
            "UPDATE answers SET content = $1, raw_content = $2 WHERE id = $3 AND author = $4 RETURNING id",
        )
        .bind(&update_answer.content)
        .bind(&update_answer.raw_content)
        .bind(&update_answer.answer_id)
        .bind(&update_answer.author)
        .map(|row: sqlx::postgres::PgRow| -> uuid::Uuid { row.get("id") })
        .fetch_one(&self.connection)
        .await
        {
            Ok(id) => id,
            Err(e) => return Err(e),
        };

        let answer_author = self.get_an_answer_from_db(None, a_id).await?;

        Ok(answer_author)
    }
}
```

Just like for questions, answers also have some simple operations which we are familiar with now. There are also some simple operations for tags:

```rust
use sqlx::Row;

impl crate::store::Store {
    #[tracing::instrument(name = "get_tag_ids_from_db", skip(tag_names))]
    pub async fn get_tag_ids_from_db(
        &self,
        tag_names: Vec<String>,
    ) -> Result<Vec<uuid::Uuid>, sqlx::Error> {
        match sqlx::query("SELECT id FROM tags WHERE name = ANY($1)")
            .bind(&tag_names)
            .map(|row: sqlx::postgres::PgRow| -> uuid::Uuid { row.get("id") })
            .fetch_all(&self.connection)
            .await
        {
            Ok(ids) => Ok(ids),
            Err(e) => Err(e),
        }
    }
    #[tracing::instrument(name = "validate_tags", skip(tag_ids))]
    pub async fn validate_tags(&self, tag_ids: &[String]) -> Result<(), sqlx::Error> {
        if tag_ids.is_empty() {
            return Err(sqlx::Error::RowNotFound);
        }
        let rows = sqlx::query("SELECT id FROM tags WHERE id = ANY($1)")
            .bind(&tag_ids)
            .fetch_all(&self.connection)
            .await?;

        if rows.len() == tag_ids.len() {
            Ok(())
        } else {
            Err(sqlx::Error::RowNotFound)
        }
    }
}
```

### Step 2: Slugify title and compile markdowns

As a platform that allows expressiveness, we want our users to be bold enough to ask and answer questions with either plain text or some markdowns. Compiling markdown to HTML in Rust can be done via the [pulldown-cmark][51] crate. We used it in this utility function:

```rust
// backend/src/utils/qa.rs
use itertools::Itertools;

pub async fn slugify(title: &str) -> String {
    let regex = regex::Regex::new(r#"(?m)[\p{P}\p{S}]"#).unwrap();
    let result = regex.replace_all(title, "");
    result
        .to_ascii_lowercase()
        .split_ascii_whitespace()
        .join("-")
}

#[tracing::instrument(name = "Convert markdown to HTML", skip(text))]
pub async fn convert_markdown_to_html(text: &str) -> String {
    let mut options = pulldown_cmark::Options::empty();
    options.insert(pulldown_cmark::Options::ENABLE_FOOTNOTES);
    options.insert(pulldown_cmark::Options::ENABLE_TASKLISTS);
    options.insert(pulldown_cmark::Options::ENABLE_HEADING_ATTRIBUTES);
    options.insert(pulldown_cmark::Options::ENABLE_SMART_PUNCTUATION);
    options.insert(pulldown_cmark::Options::ENABLE_TABLES);
    options.insert(pulldown_cmark::Options::ENABLE_STRIKETHROUGH);
    let parser = pulldown_cmark::Parser::new_ext(text, options);
    let mut html_output: String = String::with_capacity(text.len() * 3 / 2);
    pulldown_cmark::html::push_html(&mut html_output, parser);

    html_output
}
```

In `convert_markdown_to_html`, we enabled a lot of advanced markdown commands including the [Github flavoured tables][52], [Github flavoured task lists][53] and [strikethrough][54].

We also used the avenue to sluggify the question title. We used [`regex`][55] to fish out and replace all occurrences of punctuation and symbol characters with an empty string and using the [itertools][56] crate, we joined the words back together into a single string, where each word is separated by a hyphen ("-").

That's it for this article. We'll utilize these things in the next one!

[50]: https://dev.to/sirneij/cryptoflow-building-a-secure-and-scalable-system-with-axum-and-sveltekit-part-0-mn5 "CryptoFlow: Building a secure and scalable system with Axum and SvelteKit - Part 0"
[51]: https://crates.io/crates/pulldown-cmark "A pull parser for CommonMark"
[52]: https://github.github.com/gfm/#tables-extension- "Tables (extension) "
[53]: https://github.github.com/gfm/#task-list-items-extension- "Task list items (extension)"
[54]: https://github.github.com/gfm/#strikethrough-extension- "Strikethrough (extension)"
[55]: https://crates.io/crates/regex "An implementation of regular expressions for Rust. This implementation uses finite automata and guarantees linear time matching on all inputs."
[56]: https://crates.io/crates/itertools "Extra iterator adaptors, iterator methods, free functions, and macros."

# CryptoFlow: Building a secure and scalable system with Axum and SvelteKit - Part 4

## Introduction

[Part 3][60] laid some foundations and this part will build on them to build a CRUD system for questions and answers.

## Source code

The source code for this series is hosted on GitHub via:

[github.com/Sirneij/cryptoflow][100]

## Implementation

### Step: "Ask Question" feature

To hit the ground running, we want to implement the endpoint where questions about cryptocurrency can be asked. Keeping in mind the idea of modularity, all the handlers about the Q&A service will be domiciled in `backend/src/routes/qa`. As seen before, the module will only expose the composed routes of the handlers.

Now to `backend/src/routes/qa/ask.rs`:

```rust
use crate::models::{CreateQuestion, NewQuestion};
use crate::startup::AppState;
use crate::utils::{CustomAppError, CustomAppJson};
use axum::{extract::State, response::IntoResponse};
use axum_extra::extract::PrivateCookieJar;

#[axum::debug_handler]
#[tracing::instrument(name = "ask_question", skip(state, cookies, new_question))]
pub async fn ask_question(
    State(state): State<AppState>,
    cookies: PrivateCookieJar,
    CustomAppJson(new_question): CustomAppJson<NewQuestion>,
) -> Result<impl IntoResponse, CustomAppError> {
    if new_question.title.is_empty() {
        return Err(CustomAppError::from((
            "Title cannot be empty".to_string(),
            crate::utils::ErrorContext::BadRequest,
        )));
    }

    if new_question.content.is_empty() {
        return Err(CustomAppError::from((
            "Content cannot be empty".to_string(),
            crate::utils::ErrorContext::BadRequest,
        )));
    }

    if new_question.tags.is_empty() {
        return Err(CustomAppError::from((
            "Tags cannot be empty".to_string(),
            crate::utils::ErrorContext::BadRequest,
        )));
    }

    // Process tags
    let mut tag_ids: Vec<String> = new_question
        .tags
        .split(",")
        .map(|s| s.trim().to_string())
        .collect();

    // Check if tags are more than 4
    if tag_ids.len() > 4 {
        return Err(CustomAppError::from((
            "Tags cannot be more than 4".to_string(),
            crate::utils::ErrorContext::BadRequest,
        )));
    }

    // Sort and deduplicate tags
    tag_ids.sort();
    tag_ids.dedup();

    // Validate tags
    state.db_store.validate_tags(&tag_ids).await?;

    // Get author id from session
    let (user_uuid, _) =
        crate::utils::get_user_id_from_session(&cookies, &state.redis_store, false).await?;

    // Create question
    let create_question = CreateQuestion {
        slug: crate::utils::slugify(&new_question.title).await,
        title: new_question.title,
        content: crate::utils::convert_markdown_to_html(&new_question.content).await,
        raw_content: new_question.content,
        author: user_uuid,
        tags: tag_ids,
    };

    let question = state
        .db_store
        .create_question_in_db(create_question)
        .await?;

    Ok(axum::Json(question).into_response())
}
```

Pretty basic! We just validated the `title`, `content`, and `tags` supplied by the client. If any is empty, we return appropriate messages. Since we expect the tags to be a single string separated by commas like `"bitcoin, bnb, dogecoin"`, we tried to turn that into a vector of strings, that is `["bitcoin", "bnb", "dogecoin"]`, which will then be revalidated (shouldn't be more than 4), sorted and deduplicated. Sorting is required for deduplication to work. Then an instance is created which is sent to the database.

### Step 2: Question management

Having created a question, we still need other ways to manage it such as retrieving, updating and even deleting it. These management handlers are:

```rust
// backend/src/routes/qa/questions.rs
use crate::{
    startup::AppState,
    utils::{CustomAppError, CustomAppJson, ErrorContext},
};
use axum::{
    extract::{Path, State},
    http::StatusCode,
    response::IntoResponse,
};
use axum_extra::extract::PrivateCookieJar;

#[axum::debug_handler]
#[tracing::instrument(name = "all_question", skip(state))]
pub async fn all_questions(
    State(state): State<AppState>,
) -> Result<impl IntoResponse, CustomAppError> {
    let questions = state.db_store.get_all_questions_from_db().await?;

    Ok(axum::Json(questions).into_response())
}

#[axum::debug_handler]
#[tracing::instrument(name = "get_question", skip(state))]
pub async fn get_question(
    Path(question_id): Path<uuid::Uuid>,
    State(state): State<AppState>,
) -> Result<impl IntoResponse, CustomAppError> {
    let question = state
        .db_store
        .get_question_from_db(None, question_id)
        .await?;

    Ok(axum::Json(question).into_response())
}

#[axum::debug_handler]
#[tracing::instrument(name = "delete_a_question", skip(state))]
pub async fn delete_a_question(
    Path(question_id): Path<uuid::Uuid>,
    State(state): State<AppState>,
    cookies: PrivateCookieJar,
) -> Result<impl IntoResponse, CustomAppError> {
    // Get author id from session
    let (user_uuid, _) =
        crate::utils::get_user_id_from_session(&cookies, &state.redis_store, false).await?;

    state
        .db_store
        .delete_question_from_db(user_uuid, question_id)
        .await
        .map_err(|_| {
            CustomAppError::from((
                "Failed to delete question and it's most probably due to not being authorized"
                    .to_string(),
                ErrorContext::UnauthorizedAccess,
            ))
        })?;

    let response = crate::utils::SuccessResponse {
        message: "Question deleted successfully".to_string(),
        status_code: StatusCode::NO_CONTENT.as_u16(),
    };

    Ok(response.into_response())
}

#[axum::debug_handler]
#[tracing::instrument(name = "update_a_question", skip(state))]
pub async fn update_a_question(
    Path(question_id): Path<uuid::Uuid>,
    State(state): State<AppState>,
    cookies: PrivateCookieJar,
    CustomAppJson(update_question): CustomAppJson<crate::models::UpdateQuestion>,
) -> Result<impl IntoResponse, CustomAppError> {
    // Get author id from session
    let (user_uuid, _) =
        crate::utils::get_user_id_from_session(&cookies, &state.redis_store, false).await?;

    // Extract tags from update_question
    let mut tag_ids: Vec<String> = update_question
        .tags
        .split(",")
        .map(|s| s.trim().to_string())
        .collect();

    // Check if tags are more than 4
    if tag_ids.len() > 4 {
        return Err(CustomAppError::from((
            "Tags cannot be more than 4".to_string(),
            crate::utils::ErrorContext::BadRequest,
        )));
    }

    // Sort and deduplicate tags
    tag_ids.sort();
    tag_ids.dedup();

    // Create a question out of update_question
    let new_update_question = crate::models::CreateQuestion {
        slug: crate::utils::slugify(&update_question.title).await,
        title: update_question.title,
        content: crate::utils::convert_markdown_to_html(&update_question.content).await,
        raw_content: update_question.content,
        author: user_uuid,
        tags: tag_ids,
    };

    state
        .db_store
        .update_question_in_db(question_id, new_update_question)
        .await
        .map_err(|_| {
            CustomAppError::from((
                "Failed to update question and it's most probably due to not being authorized"
                    .to_string(),
                ErrorContext::UnauthorizedAccess,
            ))
        })?;

    let response = crate::utils::SuccessResponse {
        message: "Question updated successfully".to_string(),
        status_code: StatusCode::NO_CONTENT.as_u16(),
    };

    Ok(response.into_response())
}
```

It's just the basic stuff. The database operation abstractions previously written made everything else basic.

### Step 3: Answering a question

Questions are fun but getting answers is merrier. People ask questions so they can have answers to them. We will implement the handler for this here:

```rust
// backend/src/routes/qa/answer.rs
use crate::{
    models::{CreateAnswer, NewAnswer},
    startup::AppState,
    utils::{CustomAppError, CustomAppJson},
};
use axum::{
    extract::{Path, State},
    response::IntoResponse,
};
use axum_extra::extract::PrivateCookieJar;

#[axum::debug_handler]
#[tracing::instrument(name = "answer_question", skip(state))]
pub async fn answer_question(
    Path(question_id): Path<uuid::Uuid>,
    State(state): State<AppState>,
    cookies: PrivateCookieJar,
    CustomAppJson(new_answer): CustomAppJson<NewAnswer>,
) -> Result<impl IntoResponse, CustomAppError> {
    // Get author id from session
    let (user_uuid, _) =
        crate::utils::get_user_id_from_session(&cookies, &state.redis_store, false).await?;

    // Create answer
    let create_answer = CreateAnswer {
        content: crate::utils::convert_markdown_to_html(&new_answer.content).await,
        raw_content: new_answer.content,
        author: user_uuid,
        question: question_id,
    };

    let answer = state.db_store.create_answer_in_db(create_answer).await?;

    Ok(axum::Json(answer).into_response())
}
```

As usual, it is basic! Things get basic as soon as there are the "low-level" stuff has been abstracted away!

### Step 4: Answers management

Just like questions, answers need to be managed as well. We provide the basic handlers for those here:

```rust
// backend/src/routes/qa/answers.rs
use crate::{
    models::{NewAnswer, UpdateAnswer},
    startup::AppState,
    utils::{CustomAppError, CustomAppJson, ErrorContext},
};
use axum::{
    extract::{Path, State},
    http::StatusCode,
    response::IntoResponse,
};
use axum_extra::extract::PrivateCookieJar;

#[axum::debug_handler]
#[tracing::instrument(name = "question_answers", skip(state))]
pub async fn question_answers(
    Path(question_id): Path<uuid::Uuid>,
    State(state): State<AppState>,
) -> Result<impl IntoResponse, CustomAppError> {
    let answers = state
        .db_store
        .get_answers_from_db(None, question_id)
        .await?;

    Ok(axum::Json(answers).into_response())
}

#[axum::debug_handler]
#[tracing::instrument(name = "delete_an_answer", skip(state))]
pub async fn delete_an_answer(
    Path(answer_id): Path<uuid::Uuid>,
    State(state): State<AppState>,
    cookies: PrivateCookieJar,
) -> Result<impl IntoResponse, CustomAppError> {
    // Get author id from session
    let (user_uuid, _) =
        crate::utils::get_user_id_from_session(&cookies, &state.redis_store, false).await?;

    state
        .db_store
        .delete_answer_from_db(user_uuid, answer_id)
        .await
        .map_err(|_| {
            CustomAppError::from((
                "Failed to delete answer and it's most probably due to not being authorized"
                    .to_string(),
                ErrorContext::UnauthorizedAccess,
            ))
        })?;

    Ok(crate::utils::SuccessResponse {
        message: "Answer deleted successfully".to_string(),
        status_code: StatusCode::NO_CONTENT.as_u16(),
    }
    .into_response())
}

#[axum::debug_handler]
#[tracing::instrument(name = "update_answer", skip(state))]
pub async fn update_answer(
    Path(answer_id): Path<uuid::Uuid>,
    State(state): State<AppState>,
    cookies: PrivateCookieJar,
    CustomAppJson(new_answer): CustomAppJson<NewAnswer>,
) -> Result<impl IntoResponse, CustomAppError> {
    // Get author id from session
    let (user_uuid, _) =
        crate::utils::get_user_id_from_session(&cookies, &state.redis_store, false).await?;

    let new_answer = UpdateAnswer {
        content: crate::utils::convert_markdown_to_html(&new_answer.content).await,
        raw_content: new_answer.content,
        author: user_uuid,
        answer_id,
    };

    let answer = state
        .db_store
        .update_answer_in_db(new_answer)
        .await
        .map_err(|_| {
            CustomAppError::from((
                "Failed to update answer and it's most probably due to not being authorized"
                    .to_string(),
                ErrorContext::UnauthorizedAccess,
            ))
        })?;

    Ok(CustomAppJson(answer).into_response())
}
```

### Step 5: Build the routes

Having implemented all the handlers, let's tie all of them up to their respective URIs:

```rust
// backend/src/routes/qa/mod.rs
use crate::utils::validate_authentication_session;
use axum::{
    routing::{delete, get, post},
    Router,
};

mod answer;
mod answers;
mod ask;
mod questions;

pub fn qa_routes(state: crate::startup::AppState) -> Router<crate::startup::AppState> {
    Router::new()
        .route("/ask", post(ask::ask_question))
        .route("/answer/:question_id", post(answer::answer_question))
        .route(
            "/questions/:question_id",
            delete(questions::delete_a_question).patch(questions::update_a_question),
        )
        .route(
            "/answers/:answer_id",
            delete(answers::delete_an_answer).patch(answers::update_answer),
        )
        .route_layer(axum::middleware::from_fn_with_state(
            state.clone(),
            validate_authentication_session,
        ))
        .route("/questions", get(questions::all_questions))
        .route("/questions/:question_id", get(questions::get_question))
        .route(
            "/questions/:question_id/answers",
            get(answers::question_answers),
        )
}
```

Notice how we were able to join multiple HTTP methods to a single route? It's powerful, I must confess!

As usual, we need to `nest` this group of routes to our main route:

```rust
// backend/src/startup.rs
...

async fn run(
    listener: tokio::net::TcpListener,
    store: crate::store::Store,
    settings: crate::settings::Settings,
) {
    ...
    // build our application with a route
    let app = axum::Router::new()
        ...
        .nest("/api/qa", routes::qa_routes(app_state.clone()))
        ...
}
...
```

Our Q&A service is now up!!!

### Step 6: Integrating [CoinGecko API][61]

At the start, we said our system would use [CoinGecko APIs][61] (in case you use their premium services, kindly use my referral code `CGSIRNEIJ`) to get the "real-time" price of cryptocurrencies. To make it truly real-time, we would have used WebSockets. However, for now, let's just be getting the data from the API on refresh. Later, we'll talk about how to have a truly "real-time" with WebSockets. The handlers for getting the actual prices will be implemented next. We will also implement getting the list of coins from the API periodically (every 24 hours). Let's start it:

```rust
// backend/src/utils/crypto.rs
use reqwest;
use serde_json::Value;
use std::collections::HashMap;

#[derive(Debug, serde::Serialize, serde::Deserialize)]
pub struct CryptoPrice {
    name: String,
    price: f64,
}

pub type CryptoPrices = Vec<CryptoPrice>;

#[tracing::instrument(name = "get_crypto_prices")]
pub async fn get_crypto_prices(
    cryptos: String,
    currency: &str,
) -> Result<CryptoPrices, reqwest::Error> {
    let url = format!(
        "https://api.coingecko.com/api/v3/simple/price?ids={}&vs_currencies={}",
        cryptos,
        currency.to_lowercase()
    );

    let response: HashMap<String, Value> = reqwest::get(&url)
        .await?
        .json::<HashMap<String, Value>>()
        .await?;

    let mut prices = CryptoPrices::new();
    for (name, data) in response {
        if let Some(price) = data.get("usd").and_then(|v| v.as_f64()) {
            prices.push(CryptoPrice { name, price });
        }
    }

    Ok(prices)
}
```

Using the `reqwest` crate and `v3/simple/price?ids={}&vs_currencies={}` endpoint, we wrote a utility function that abstracts away retrieving the current prices of the cryptocurrencies. It's "mostly" hard-coded for now but it's okay.

Next is its use in a handler:

```rust
// backend/src/routes/crypto/price.rs
use crate::utils::{get_crypto_prices, CryptoPrices, CustomAppError, CustomAppJson};
use axum::extract::Query;

#[derive(serde::Deserialize, Debug)]
pub struct CryptoPriceRequest {
    tags: String,
    currency: String,
}

#[axum::debug_handler]
#[tracing::instrument(name = "crypto_price_handler")]
pub async fn crypto_price_handler(
    Query(crypto_req): Query<CryptoPriceRequest>,
) -> Result<CustomAppJson<CryptoPrices>, CustomAppError> {
    // Call the get_crypto_prices function with the tags
    let prices = get_crypto_prices(crypto_req.tags, &crypto_req.currency)
        .await
        .map_err(CustomAppError::from)?;

    // Return the prices wrapped in CustomAppJson
    Ok(CustomAppJson(prices))
}
```

As it's the norm so far, we put all direct crypto-related logic in a routes submodule. The handler above just helps to make available the prices of a list of coins in `USD`.

```rust
// backend/src/routes/crypto/coins.rs
use crate::{startup::AppState, utils::CustomAppError};
use axum::{extract::State, response::IntoResponse};

#[axum::debug_handler]
#[tracing::instrument(name = "all_coins", skip(state))]
pub async fn all_coins(State(state): State<AppState>) -> Result<impl IntoResponse, CustomAppError> {
    let coins = state.db_store.get_all_coins_from_db().await?;

    Ok(axum::Json(coins).into_response())
}
```

This exposes the list of coins supported by [CoinGecko API][61]. We implement the `get_all_coins_from_db` utility method and the other one below:

```rust
// backend/src/store/crypto.rs
impl crate::store::Store {
    #[tracing::instrument(name = "get_all_coins_from_db", skip(self))]
    pub async fn get_all_coins_from_db(&self) -> Result<Vec<crate::models::Tag>, sqlx::Error> {
        let tags = sqlx::query_as::<_, crate::models::Tag>("SELECT * FROM tags")
            .fetch_all(&self.connection)
            .await?;

        Ok(tags)
    }
    pub async fn update_coins(&self) {
        let url = "https://api.coingecko.com/api/v3/coins/list";

        match reqwest::get(url).await {
            Ok(response) => match response.json::<Vec<crate::models::Tag>>().await {
                Ok(coins) => {
                    let ids: Vec<String> = coins.iter().map(|c| c.id.clone()).collect();
                    let names: Vec<String> = coins.iter().map(|c| c.name.clone()).collect();
                    let symbols: Vec<String> = coins.iter().map(|c| c.symbol.clone()).collect();

                    let query = sqlx::query(
                        "INSERT INTO tags (id, name, symbol) SELECT * FROM UNNEST($1::text[], $2::text[], $3::text[]) ON CONFLICT (id) DO UPDATE SET name = EXCLUDED.name, symbol = EXCLUDED.symbol",
                    )
                    .bind(&ids)
                    .bind(&names)
                    .bind(&symbols);

                    if let Err(e) = query.execute(&self.connection).await {
                        tracing::error!("Failed to update coins: {}", e);
                    }
                }
                Err(e) => tracing::error!("Failed to parse coins from response: {}", e),
            },
            Err(e) => tracing::error!("Failed to fetch coins from CoinGecko: {}", e),
        }
    }
}
```

The first utility method returns all the tags (coins) we have saved. The second one uses [CoinGecko API][61]'s `/api/v3/coins/list` route to retrieve the updated list of coins supported on the platform (it's a lot by the way!) and save them in the database!

### Step 7: Bundle up `crypto` routes

Let's build the few crypto-related routes we have so far and then `nest` the outcome to the main route:

```rust
// backend/src/routes/crypto/mod.rs
use axum::{routing::get, Router};

mod coins;
mod price;
mod prices;

pub fn crypto_routes() -> Router<crate::startup::AppState> {
    Router::new()
        .route("/prices", get(price::crypto_price_handler))
        .route("/coins", get(coins::all_coins))
}
```

We will now nest the route to the main one. While doing that, we will also spawn a periodic tokio task, running in the background, to fetch and update the list of coins from CoinGecko:

```rust
// backend/src/startup.rs
...
use tokio::time::{sleep, Duration};

...

impl Application {
    ...
    pub async fn build(
        settings: crate::settings::Settings,
        test_pool: Option<sqlx::postgres::PgPool>,
    ) -> Result<Self, std::io::Error> {
        ...
        let store_for_update = store.clone();

        // Update coins
        tokio::spawn(async move {
            loop {
                store_for_update.update_coins().await;
                sleep(Duration::from_secs(
                    settings.interval_of_coin_update * 60 * 60,
                ))
                .await;
            }
        });

        ...
    }
}

async fn run(
    listener: tokio::net::TcpListener,
    store: crate::store::Store,
    settings: crate::settings::Settings,
) {
    ...
    // build our application with a route
    let app = axum::Router::new()
        ...
        .nest("/api/crypto", routes::crypto_routes())
        ...
}
...
```

We need to clone the store because if not, the code won't compile since we'd have succeeded in moving the `store` struct out of scope for `run` to use. Using `tokio`'s sleep and duration, we were able to schedule a 24-hour periodic task that fetches the coin's list from an external API.

With that, we have completed working with the main features of the backend service. There is still some stuff to do but we're fine for now. I may add one article more on the backend stuff or update some here later without having to write a whole new article.

In the next few articles, we'll see how parts of the frontend are written using SvelteKit. Hope to catch you up later!

[60]: https://dev.to/sirneij/cryptoflow-building-a-secure-and-scalable-system-with-axum-and-sveltekit-part-3-1bh5 "CryptoFlow: Building a secure and scalable system with Axum and SvelteKit - Part 3"
[61]: https://www.coingecko.com/en/api "Explore the API"

# CryptoFlow: Building a secure and scalable system with Axum and SvelteKit - Part 5

## Introduction

From [part 0][70] to [part 4][71], we built out `CryptoFlow`'s backend service. Though we can quickly use Postman, VS Code's ThunderClient or automated tests to see the endpoints working easily, this isn't all we want. We want to actively interact with the backend service via some intuitive user interface. Also, a layman wouldn't be able to "consume" the service we've built in the last parts. This article introduces building out the user interface of the system. We will be using [SvelteKit][72], a framework that streamlines web development, and [TailwindCSS][73], the utility-first CSS framework. Let's dig in!

## Source code

The source code for this series is hosted on GitHub via:

[github.com/Sirneij/cryptoflow][100]

## Implementation

### Step 1: Set up the frontend app

To ease the entire process, kindly head on to [creating a project][74]. I named this `frontend`. After that, proceed to [install tailwind CSS with sveltekit][75]. Kindly do this before moving to the next step.

Since we want the frontend application to be served on port `3000`, head on to `frontend/package.json` and update the `dev` script to:

```json
...
"dev": "vite dev --port 3000",
...
```

We also need to get the backend URL for server communication. Depending on your backend domain or where you host the backend code, this can change. We will use a `.env` file at the root of the `frontend` app:

```
VITE_BASE_API_URI_DEV=http://127.0.0.1:8008/api
VITE_BASE_API_URI_PROD=
```

Normally, variables in `.env` files served by vite should have the `VITE_` prefix. You can change the value to the URL of your local web server (axum backend built in the last few articles). The `.env` file has two variables that store the backend APIs depending on whether it's in a development or production environment.

Retrieving and using this environment variable can be made seamless by exporting it to a `.js` file. In SvelteKit, I like to do this in `frontend/src/lib/utils/constants.js`:

```js
export const BASE_API_URI = import.meta.env.DEV
  ? import.meta.env.VITE_BASE_API_URI_DEV
  : import.meta.env.VITE_BASE_API_URI_PROD;
```

That automatically updates `BASE_API_URI` from the `.env` file.

### Step 2: Getting the app layout

As earlier stated, we'll use a lot of [TailwindCSS][73] to style the web interface. I have already done that and won't be going into the nitty-gritty of the interface or how to use [TailwindCSS][73]. That said, let's build out how the general outlook of the app will be. Open up `frontend/src/routes/+layout.svelte`:

```html
<script>
  import "../app.css";
  import Header from "$lib/components/header/Header.svelte";
  import Footer from "$lib/components/footer/Footer.svelte";
  import Transition from "$lib/components/Transition.svelte";

  export let data;
</script>

<Transition key="{data.url}" duration="{600}">
  <header class="sticky top-0 z-50 bg-[#0a0a0a]">
    <header />
  </header>

  <slot />

  <footer />
</Transition>
```

The first import, `import '../app.css'`, was gotten from [setting up tailwind CSS with sveltekit][75]. Others were components written. I won't show codes for all the components, however, the `Transition` component looks like this:

```html
<script>
	import { slide } from 'svelte/transition';
	/** @type {string} */
	export let key;

	/** @type {number} */
	export let duration = 300;
</script>

{#key key}
	<div in:slide={{ duration, delay: duration }} out:slide={{ duration }}>
		<slot />
	</div>
{/key}
```

The component simply allows smooth page transactions. Just to have nice effects while navigating pages. The transition requires a key which should be distinct for each page. A simple and intuitive thing that comes to mind is the page's URL which is generally unique. To make available the page's URL, we need to expose it in `+layout.js`:

```js
// frontend/src/routes/+layout.js
/** @type {import('./$types').LayoutLoad} */
export async function load({ fetch, url, data }) {
  const { user } = data;
  return { fetch, url: url.pathname, user };
}
```

We didn't only expose the URL, `fetch` and `user` were also exposed. `fetch` will be used to make some requests to the server later on. I always prefer using the `fetch` API provided by SvelteKit which extends the normal version of the API. `user` makes available the data of the currently logged-in user. How do we get it? We'll use the power of the [handle][6] method of SvelteKit's server-side hook:

```js
// frontend/src/hooks.server.js
import { BASE_API_URI } from "$lib/utils/constants";

/** @type {import('@sveltejs/kit').Handle} */
export async function handle({ event, resolve }) {
  if (event.locals.user) {
    // if there is already a user  in session load page as normal
    return await resolve(event);
  }
  // get cookies from browser
  const session = event.cookies.get("cryptoflow-sessionid");

  if (!session) {
    // if there is no session load page as normal
    return await resolve(event);
  }

  // find the user based on the session
  const res = await event.fetch(`${BASE_API_URI}/users/current`, {
    credentials: "include",
    headers: {
      Cookie: `sessionid=${session}`,
    },
  });

  if (!res.ok) {
    // if there is no session load page as normal
    return await resolve(event);
  }

  // if `user` exists set `events.local`
  const response = await res.json();

  event.locals.user = response;

  // load page as normal
  return await resolve(event);
}
```

The server-side [handle][76] hook "runs every time the SvelteKit server receives a request and determines the response". Normally, the function takes the `event` and `resolve` arguments which represent the incoming request and renders the routes alongside its response respectively. The `event` object has `locals` as one of its properties. We can use the `Locals` TypeScript's `interface` to hold some data. The data it holds can be accessed and subsequently exposed in the `load` functions of `+page/layout.server.js/ts`. The comments in `frontend/src/hooks.server.js` do enough justice to what it does. To satisfy `JsDoc` or `TypeScript` requirements, we need to add the `user` property to the `Locals` interface:

```ts
// frontend/src/app.d.ts

interface User {
	email: string;
	first_name: string;
	last_name: string;
	id: string;
	is_staff: boolean;
	is_active: boolean;
	thumbnail: string;
	is_superuser: boolean;
	date_joined: string;
}
...
declare global {
	namespace App {
		...
		interface Locals {
			user: User;
		}
		...
	}
}
...
```

Making the `user` available to all routes is our aim. A good place to ensure this is the `+layout.server.js` file which can help propagate the user's data.

```js
// frontend/src/routes/+layout.server.js
/** @type {import('./$types').LayoutServerLoad} */
export async function load({ locals }) {
  return {
    user: locals.user,
  };
}
```

The exposed `user` object was what we retrieved from the `data` argument in `frontend/src/routes/+layout.js`. With that, we can now access the user's data on any page via the `data` property of the [`page`][77] store.

The other components imported in `frontend/src/routes/+layout.svelte` are just some simple Tailwind CSS-styled HTML documents.

### Step 3: Utility components and functions

In the spirit of keeping most of the tiny details out of the way, we will go through some simple components and functions that will be used in subsequent articles. The first we will see is `frontend/src/lib/utils/helpers.js`:

```js
// frontend/src/lib/utils/helpers.js
// @ts-nocheck
import { quintOut } from "svelte/easing";
import { crossfade } from "svelte/transition";

export const [send, receive] = crossfade({
  duration: (d) => Math.sqrt(d * 200),

  // eslint-disable-next-line no-unused-vars
  fallback(node, params) {
    const style = getComputedStyle(node);
    const transform = style.transform === "none" ? "" : style.transform;

    return {
      duration: 600,
      easing: quintOut,
      css: (t) => `
                transform: ${transform} scale(${t});
                opacity: ${t}
            `,
    };
  },
});

/**
 * Validates an email field
 * @file lib/utils/helpers/input.validation.ts
 * @param {string} email - The email to validate
 */
export const isValidEmail = (email) => {
  const EMAIL_REGEX =
    /[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?/;
  return EMAIL_REGEX.test(email.trim());
};
/**
 * Validates a strong password field
 * @file lib/utils/helpers/input.validation.ts
 * @param {string} password - The password to validate
 */
export const isValidPasswordStrong = (password) => {
  const strongRegex = new RegExp(
    "^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*])(?=.{8,})"
  );

  return strongRegex.test(password.trim());
};
/**
 * Validates a medium password field
 * @file lib/utils/helpers/input.validation.ts
 * @param {string} password - The password to validate
 */
export const isValidPasswordMedium = (password) => {
  const mediumRegex = new RegExp(
    "^(((?=.*[a-z])(?=.*[A-Z]))|((?=.*[a-z])(?=.*[0-9]))|((?=.*[A-Z])(?=.*[0-9])))(?=.{6,})"
  );

  return mediumRegex.test(password.trim());
};

/**
 * Test whether or not an object is empty.
 * @param {Record<string, string>} obj - The object to test
 * @returns `true` or `false`
 */

export function isEmpty(obj) {
  for (const _i in obj) {
    return false;
  }
  return true;
}

/**
 * Handle all GET requests.
 * @file lib/utils/helpers.js
 * @param {typeof fetch} sveltekitFetch - Fetch object from sveltekit
 * @param {string} targetUrl - The URL whose resource will be fetched.
 * @param {RequestCredentials} [credentials='omit'] - Request credential. Defaults to 'omit'.
 * @param {'GET' | 'POST'} [requestMethod='GET'] - Request method. Defaults to 'GET'.
 * * @param {RequestMode | undefined} [mode='cors'] - Request mode. Defaults to 'GET'.
 */
export const getRequests = async (
  sveltekitFetch,
  targetUrl,
  credentials = "omit",
  requestMethod = "GET",
  mode = "cors"
) => {
  const headers = { "Content-Type": "application/json" };

  const requestInitOptions = {
    method: requestMethod,
    mode: mode,
    credentials: credentials,
    headers: headers,
  };

  const res = await sveltekitFetch(targetUrl, requestInitOptions);

  return res.ok && (await res.json());
};

/**
 * Get coin prices.
 * @file lib/utils/helpers.js
 * @param {typeof fetch} sveltekitFetch - Fetch object from sveltekit
 * @param {string} tags - The tags of the coins to fetch prices for.
 * @param {string} currency - The currency to fetch prices in.
 */
export const getCoinsPricesServer = async (sveltekitFetch, tags, currency) => {
  const res = await getRequests(
    sveltekitFetch,
    `/api/crypto/prices?tags=${tags}&currency=${currency}`
  );

  return res;
};

/**
 * Format price to be more readable.
 * @file lib/utils/helpers.js
 * @param {number} price - The price to format.
 */
export function formatPrice(price) {
  return price.toLocaleString(undefined, {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  });
}

const coinSymbols = {
  Bitcoin: "BTC",
  Ethereum: "ETH",
  BNB: "BNB",
  Litecoin: "LTC",
  Dogecoin: "DOGE",
  // Add other coins and their symbols here
};

/**
 * Format the coin name to be more readable.
 * @file lib/utils/helpers.js
 * @param {string} coinName - The coin name to format.
 */
export function formatCoinName(coinName) {
  // Format the name by capitalizing the first letter of each word
  const formattedName = coinName
    .toLowerCase()
    .replace(/(?:^|\s)\S/g, (a) => a.toUpperCase());

  // Return the formatted name with the coin's symbol (if available)
  return `${formattedName} (${coinSymbols[formattedName] || "N/A"})`;
}

export function timeAgo(dateString) {
  const date = new Date(dateString);
  const now = new Date();

  const secondsAgo = Math.round((now - date) / 1000);
  const minutesAgo = Math.round(secondsAgo / 60);
  const hoursAgo = Math.round(minutesAgo / 60);
  const daysAgo = Math.round(hoursAgo / 24);

  const rtf = new Intl.RelativeTimeFormat("en", { numeric: "auto" });

  if (secondsAgo < 60) {
    return rtf.format(-secondsAgo, "second");
  } else if (minutesAgo < 60) {
    return rtf.format(-minutesAgo, "minute");
  } else if (hoursAgo < 24) {
    return rtf.format(-hoursAgo, "hour");
  } else if (daysAgo < 30) {
    return rtf.format(-daysAgo, "day");
  } else {
    // Fallback to a more standard date format
    return date.toLocaleDateString();
  }
}
```

Just some random functions for fading animations; email, password and object validations; sending `GET` requests; and price (may be removed) and age formatting.

Aside from those functions, there are some other ones in `frontend/src/lib/utils/select.custom.js`:

```js
// frontend/src/lib/utils/select.custom.js
/**
 * Tag Selection and Suggestion Management.
 * Handles the logic for filtering tags based on user input, selecting tags, and displaying selected tags and suggestions.
 */

import { selectedTags } from "$lib/stores/tags.stores";
import { get } from "svelte/store";

/** @type {HTMLInputElement} */
let inputFromOutside;

/**
 * Set the input element.
 * @file $lib/utils/select.custom.ts
 * @param {HTMLInputElement} inputElement - The input element
 */
export function setInputElement(inputElement) {
  inputFromOutside = inputElement;
}

// Create a Tag type that has id, name, and symbol properties all of type string in jsdoc
/**
 * @typedef {Object} Tag
 * @property {string} id
 * @property {string} name
 * @property {string} symbol
 */

/**
 * Filter tags based on user input and display suggestions.
 * @file $lib/utils/select.custom.ts
 * @param {HTMLInputElement} tagInput - The input element
 * @param {Array<Tag>} allTags - All the tags
 */
export function filterTags(tagInput, allTags) {
  inputFromOutside = tagInput;
  const input = tagInput.value.toLowerCase();

  if (input.trim() === "") {
    clearSuggestions();
    return;
  }

  let $selectedTags = get(selectedTags);

  const suggestions = allTags.filter(
    (tag) =>
      (tag.id.toLowerCase().includes(input) ||
        tag.name.toLowerCase().includes(input)) &&
      !$selectedTags.includes(tag.id)
  );

  displaySuggestions(suggestions);
}

/**
 * Select a tag and display it.
 * @file $lib/utils/select.custom.ts
 * @param {string} tagId - The tag to select
 */
function selectTag(tagId) {
  if (!get(selectedTags).includes(tagId)) {
    // Add tag to selected tags store
    selectedTags.set([...get(selectedTags), tagId]);
    displaySelectedTags();
    inputFromOutside.value = "";
    updateInputPlaceholder();
    clearSuggestions();
  } else {
    // Optional: Provide feedback to the user that the tag is already selected
    console.log("Tag already selected");
  }
}
/**
 * Clear suggestions.
 * @file $lib/utils/select.custom.ts
 */
function clearSuggestions() {
  const container = document.getElementById("suggestions");
  // @ts-ignore
  container.innerHTML = ""; // Clear suggestions
}

/**
 * Remove a tag from the selected tags.
 * @file $lib/utils/select.custom.ts
 * @param {string} tagId - The ID of the tag to remove
 */
function removeTag(tagId) {
  let $selectedTags = get(selectedTags);
  $selectedTags = $selectedTags.filter((t) => t !== tagId);
  selectedTags.set($selectedTags);
  displaySelectedTags();
  updateInputPlaceholder();
}

/**
 * Update the input placeholder text based on the number of selected tags.
 */
function updateInputPlaceholder() {
  let $selectedTags = get(selectedTags);
  if ($selectedTags.length === 4) {
    inputFromOutside.disabled = true;
    inputFromOutside.placeholder = "Max tags reached";
  } else {
    inputFromOutside.disabled = false;
    inputFromOutside.placeholder = `Add up to ${
      4 - $selectedTags.length
    } more tags`;
  }
}

/**
 * Display suggestions to the user.
 * @file $lib/utils/select.custom.ts
 * @param {Array<Tag>} tags - The tags to display
 */
function displaySuggestions(tags) {
  /** @type {HTMLElement} */
  // @ts-ignore
  const container = document.getElementById("suggestions");
  container.innerHTML = ""; // Clear existing suggestions

  tags.forEach((tag) => {
    const div = document.createElement("div");
    div.textContent = tag.name;
    div.className = "cursor-pointer p-2 hover:bg-[#145369]";
    div.addEventListener("click", () => selectTag(tag.id)); // Attach event listener
    container.appendChild(div);
  });
}

/**
 * Display selected tags to the user.
 * @file $lib/utils/select.custom.ts
 */
export function displaySelectedTags() {
  const container = document.getElementById("selected-tags");
  // @ts-ignore
  container.innerHTML = ""; // Clear existing tags

  let $selectedTags = get(selectedTags);

  $selectedTags.forEach((tag) => {
    const span = document.createElement("span");
    span.className =
      "inline-block bg-[#145369] rounded-full px-3 py-1 text-sm font-semibold text-white mr-2 mb-2";
    span.textContent = tag;

    const removeSpan = document.createElement("span");
    removeSpan.className = "cursor-pointer text-red-500 hover:text-red-600";
    removeSpan.textContent = " x";
    removeSpan.onclick = () => removeTag(tag); // Attach event listener

    span.appendChild(removeSpan);
    // @ts-ignore
    container.appendChild(span);
  });
}
```

The appended comments say exactly what the file and functions do. There is a custom store introduced there:

```js
// frontend/src/lib/stores/tags.stores.js
import { writable } from "svelte/store";

/** @type {Array<string>} */
let tags = [];

export const selectedTags = writable(tags);
```

The next things are the simple components. We start with a responsive but simple loader:

```html
<!-- frontend/src/lib/components/Loader.svelte -->
<script>
  /** @type {number | null} */
  export let width;
  /** @type {string | null} */
  export let message;
</script>

<div class="loading">
  <p class="simple-loader" style={width ? `width: ${width}px` : ''} /> {#if
  message}
  <p>{message}</p>
  {/if}
</div>

<style>
  .loading {
    display: flex;
    align-items: center;
    /* justify-content: center; */
  }
  .loading p {
    margin-left: 0.5rem;
  }
  .simple-loader {
    --b: 20px; /* border thickness */
    --n: 15; /* number of dashes*/
    --g: 7deg; /* gap  between dashes*/
    --c: #2596be; /* the color */

    width: 40px; /* size */
    aspect-ratio: 1;
    border-radius: 50%;
    padding: 1px; /* get rid of bad outlines */
    background: conic-gradient(#0000, var(--c)) content-box;
    --_m: /* we use +/-1deg between colors to avoid jagged edges */ repeating-conic-gradient(
        #0000 0deg,
        #000 1deg calc(360deg / var(--n) - var(--g) - 1deg),
        #0000 calc(360deg / var(--n) - var(--g)) calc(360deg / var(--n))
      ), radial-gradient(farthest-side, #0000 calc(98% - var(--b)), #000 calc(100% -
              var(--b)));
    -webkit-mask: var(--_m);
    mask: var(--_m);
    -webkit-mask-composite: destination-in;
    mask-composite: intersect;
    animation: load 1s infinite steps(var(--n));
  }
  @keyframes load {
    to {
      transform: rotate(1turn);
    }
  }
</style>
```

Then comes the composable and flexible animated modal:

```html
<!-- frontend/src/lib/components/Modal.svelte -->
<script>
	import { quintOut } from 'svelte/easing';

	import { createEventDispatcher } from 'svelte';

	const modal = (/** @type {Element} */ node, { duration = 300 } = {}) => {
		const transform = getComputedStyle(node).transform;

		return {
			duration,
			easing: quintOut,
			css: (/** @type {any} */ t, /** @type {number} */ u) => {
				return `transform:
            ${transform}
            scale(${t})
            translateY(${u * -100}%)
          `;
			}
		};
	};

	const dispatch = createEventDispatcher();
	function closeModal() {
		dispatch('close', {});
	}
</script>

<div class="modal-background">
	<div transition:modal={{ duration: 1000 }} class="modal" role="dialog" aria-modal="true">
		<a title="Close" class="modal-close" on:click={closeModal} role="dialog">
			<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 384 512">
				<path
					d="M342.6 150.6c12.5-12.5 12.5-32.8 0-45.3s-32.8-12.5-45.3 0L192 210.7 86.6 105.4c-12.5-12.5-32.8-12.5-45.3 0s-12.5 32.8 0 45.3L146.7 256 41.4 361.4c-12.5 12.5-12.5 32.8 0 45.3s32.8 12.5 45.3 0L192 301.3 297.4 406.6c12.5 12.5 32.8 12.5 45.3 0s12.5-32.8 0-45.3L237.3 256 342.6 150.6z"
				/>
			</svg>
		</a>
		<div class="container">
			<slot />
		</div>
	</div>
</div>

<style>
	.modal-background {
		width: 100%;
		height: 100%;
		position: fixed;
		top: 0;
		left: 0;
		right: 0;
		bottom: 0;
		background: rgba(0, 0, 0, 0.9);
		z-index: 9999;
	}

	.modal {
		position: absolute;
		left: 50%;
		top: 50%;
		width: 40%;
		box-shadow: 0 0 10px hsl(0 0% 0% / 10%);
		transform: translate(-50%, -50%);
	}
	@media (max-width: 990px) {
		.modal {
			width: 90%;
		}
	}
	.modal-close {
		border: none;
	}

	.modal-close svg {
		display: block;
		margin-left: auto;
		margin-right: auto;
		fill: rgb(14 165 233 /1);
		transition: all 0.5s;
	}
	.modal-close:hover svg {
		fill: rgb(225 29 72);
		transform: scale(1.5);
	}
	.modal .container {
		max-height: 90vh;
		overflow-y: auto;
	}
	@media (min-width: 680px) {
		.modal .container {
			flex-direction: column;
			left: 0;
			width: 100%;
		}
	}
</style>
```

Last on the list is the error-showing component:

```html
<!-- frontend/src/lib/components/ShowError.svelte -->
<script>
	import { receive, send } from '$lib/utils/helpers';

	/** @type {any}*/
	export let form;
</script>

{#if form?.errors}
	<!-- Error Message Display -->
	{#each form?.errors as error (error.id)}
		<p
			class="text-red-500 p-3 text-center mb-4 italic"
			in:receive={{ key: error.id }}
			out:send={{ key: error.id }}
		>
			{error.message}
		</p>
	{/each}
{/if}
```

With that, we come to the end of the first start of building our application's front end!

[70]: https://dev.to/sirneij/cryptoflow-building-a-secure-and-scalable-system-with-axum-and-sveltekit-part-0-mn5 "CryptoFlow: Building a secure and scalable system with Axum and SvelteKit - Part 0"
[71]: https://dev.to/sirneij/cryptoflow-building-a-secure-and-scalable-system-with-axum-and-sveltekit-part-1-2mnn "CryptoFlow: Building a secure and scalable system with Axum and SvelteKit - Part 1"
[72]: https://kit.svelte.dev/ "web development, streamlined"
[73]: https://tailwindcss.com/ "A utility-first CSS framework packed with classes"
[74]: https://kit.svelte.dev/docs/creating-a-project "Creating a project"
[75]: https://tailwindcss.com/docs/guides/sveltekit "Install Tailwind CSS with SvelteKit"
[76]: https://kit.svelte.dev/docs/hooks#server-hooks-handle "Hooks"
[77]: https://kit.svelte.dev/docs/modules#$app-stores-page "Page store"

# CryptoFlow: Building a secure and scalable system with Axum and SvelteKit - Part 6

## Introduction

It's been a while since I last updated this series of articles. I have been away and I sincerely apologize for the abandonment. I will be completing the series by going through the frontend code and other updates I made at the backend. Let's get into it!

## Source code

The source code for this series is hosted on GitHub via:

[github.com/Sirneij/cryptoflow][100]

I also have the application live. You can interact with it [here][81]. Please note that the backend was deployed on [Render][82] which:

> Spins down a Free web service that goes 15 minutes without receiving inbound traffic. Render spins the service back up whenever it next receives a request to process. Spinning up a service takes up to a minute, which causes a noticeable delay for incoming requests until the service is back up and running. For example, a browser page load will hang temporarily.

## Implementation

### Step 1: Landing page

Our application will have a landing page where questions are listed. The page will be split into three resizable columns:

1. Left column: This will contain developer information and some coins with their rankings. The number of coins at a time will be specified by the `NUM_OF_COINS_TO_SHOW` constant which is `10` by default but can be made configurable. Every 10 seconds, the list will change.

2. Middle column: A list of questions will be housed here.

3. Right column: We will have some charts for plotting the `prices`, `market caps`, and `total volumes` of any selected coin. Making a total of three multi-line charts. Each line corresponds to each of the coins. We will provide users with two inputs where they can select up to 4 coins at a time, and the number of days they want their histories to be shown.

These entire requirements are implemented in `frontend/src/routes/+page.svelte`:

```html
<script>
  import Charts from "$lib/components/Charts.svelte";
  import { NUM_OF_COINS_TO_SHOW } from "$lib/utils/constants.js";
  import { onDestroy, onMount } from "svelte";

  export let data,
    /** @type {import('./$types').ActionData} */
    form;

  /**
   * @typedef {Object} Coin
   * @property {string} id - The id of the coin.
   * @property {string} name - The name of the coin.
   * @property {string} symbol - The symbol of the coin.
   * @property {string} image - The image of the coin.
   * @property {number} market_cap_rank - The market cap rank of the coin.
   */

  /**
   * @type {Coin[]}
   */
  let selectedCoins = [],
    /** @type {Number} */
    intervalId;

  $: ({ questions, coins } = data);

  const selectCoins = () => {
    const selectedCoinsSet = new Set();
    while (selectedCoinsSet.size < NUM_OF_COINS_TO_SHOW) {
      const randomIndex = Math.floor(Math.random() * coins.length);
      selectedCoinsSet.add(coins[randomIndex]);
    }
    selectedCoins = Array.from(selectedCoinsSet);
  };

  onMount(() => {
    selectCoins(); // Select coins immediately on mount
    intervalId = setInterval(selectCoins, 10000); // Select coins every 10 seconds
  });

  onDestroy(() => {
    clearInterval(intervalId); // Clear the interval when the component is destroyed
  });
</script>

<div class="flex flex-col md:flex-row text-[#efefef]">
  <!-- Left Column for Tags -->
  <div class="hidden md:block md:w-1/4 p-4 resize overflow-auto">
    <!-- Developer Profile Card -->
    <div
      class="bg-[#041014] hover:bg-black border border-black hover:border-[#145369] rounded-lg shadow p-4 mb-1"
    >
      <img
        src="https://media.licdn.com/dms/image/D4D03AQElygM4We8kqA/profile-displayphoto-shrink_800_800/0/1681662853733?e=1721865600&v=beta&t=idb1YHHzZbXHJ1MxC4Ol2ZnnbyCHq6GDtjzTzGkziLQ"
        alt="Developer"
        class="rounded-full w-24 h-24 mx-auto mb-3"
      />
      <h3 class="text-center text-xl font-bold mb-2">John O. Idogun</h3>
      <a
        href="https://github.com/sirneij"
        class="text-center text-blue-500 block mb-2"
      >
        @SirNeij
      </a>
      <p class="text-center">Developer & Creator of CryptoFlow</p>
    </div>
    <div
      class="bg-[#041014] p-6 rounded-lg shadow mb-6 hover:bg-black border border-black hover:border-[#145369]"
    >
      <h2 class="text-xl font-semibold mb-4">Coin ranks</h2>
      {#each selectedCoins as coin (coin.id)}
      <div
        class="flex items-center justify-between mb-2 border-b border-[#0a0a0a] hover:bg-[#041014] px-3 py-1"
      >
        <div class="flex items-center">
          <img
            class="w-8 h-8 rounded-full mr-2 transition-transform duration-500 ease-in-out transform hover:rotate-180"
            src="{coin.image}"
            alt="{coin.name}"
          />
          <span class="mr-2">{coin.name}</span>
        </div>
        <span
          class="inline-block bg-blue-500 text-white text-xs px-2 rounded-full uppercase font-semibold tracking-wide"
        >
          #{coin.market_cap_rank}
        </span>
      </div>
      {/each}
    </div>
  </div>

  <div class="md:w-5/12 py-4 px-2 resize overflow-auto">
    {#if questions} {#each questions as question (question.id)}
    <div
      class="
				bg-[#041014] mb-1 rounded-lg shadow hover:bg-black border border-black hover:border-[#145369]"
    >
      <div class="p-4">
        <a
          href="/questions/{question.id}"
          class="text-xl font-semibold hover:text-[#2596be]"
        >
          {question.title}
        </a>
        <!-- <p class="mt-2">{article.description}</p> -->
        <div class="mt-3 flex flex-wrap">
          {#each question.tags as tag}
          <span
            class="mr-2 mb-2 px-3 py-1 text-sm bg-[#041014] border border-[#145369] hover:border-[#2596be] rounded"
          >
            {tag.name}
          </span>
          {/each}
        </div>
      </div>
    </div>
    {/each} {/if}
  </div>

  <!-- Right Column for Charts -->
  <div class="hidden md:block md:w-1/3 px-2 py-4 resize overflow-auto">
    <div
      class="bg-[#041014] rounded-lg shadow p-4 hover:bg-black border border-black hover:border-[#145369]"
    >
      <h2 class="text-xl font-semibold mb-4">Charts</h2>
      <Charts {coins} {form} />
    </div>
  </div>
</div>
```

To select 10 unique coins every 10 seconds, we randomly get them from the `coins` data and use `Set` to ensure no duplication is permitted. This is what `selectCoins` is about. As the DOM gets loaded, we call this function and then use `setInterval` for the periodic and automatic selection. We also ensure the interval is destroyed when we navigate out of the page for memory safety reasons.

For the charts, there is a component, `Charts`, that handles the logic:

```html
<!-- frontend/src/lib/components/Charts.svelte -->
<script>
	import { applyAction, enhance } from '$app/forms';
	import { notification } from '$lib/stores/notification.store';
	import ShowError from './ShowError.svelte';
	import Loader from './Loader.svelte';
	import { fly } from 'svelte/transition';
	import { onMount } from 'svelte';
	import Chart from 'chart.js/auto';
	import 'chartjs-adapter-moment';
	import { chartConfig, handleZoom } from '$lib/utils/helpers';
	import TagCoin from './inputs/TagCoin.svelte';

	export let coins,
		/** @type {import('../../routes/$types').ActionData} */
		form;

	/** @type {HTMLInputElement} */
	let tagInput,
		/** @type {HTMLCanvasElement} */
		priceChartContainer,
		/** @type {HTMLCanvasElement} */
		marketCapChartContainer,
		/** @type {HTMLCanvasElement} */
		totalVolumeChartContainer,
		fetching = false,
		rendered = false,
		/**
		 * @typedef {Object} CryptoData
		 * @property {Array<Number>} prices - The price data
		 * @property {Array<Number>} market_caps - The market cap data
		 * @property {Array<Number>} total_volumes - The total volume data
		 */

		/**
		 * @typedef {Object.<String, CryptoData>} CryptoDataSet
		 */

		/** @type {CryptoDataSet} */
		plotData = {},
		/** @type {CanvasRenderingContext2D | null} */

		context,
		/** @type {Chart<"line", { x: Date; y: number; }[], unknown>} */
		priceChart,
		/** @type {Chart<"line", { x: Date; y: number; }[], unknown>} */
		marketCapChart,
		/** @type {Chart<"line", { x: Date; y: number; }[], unknown>} */
		totalVolumeChart,
		/** @type {CanvasRenderingContext2D|null} */
		priceContext,
		/** @type {CanvasRenderingContext2D|null} */
		marketCapContext,
		/** @type {CanvasRenderingContext2D|null} */
		totalVolumeContext;

	/** @type {import('../../routes/$types').SubmitFunction}*/
	const handleCoinDataFetch = async () => {
		fetching = true;
		return async ({ result }) => {
			fetching = false;
			if (result.type === 'success') {
				$notification = { message: 'Coin data fetched successfully', colorName: 'blue' };

				if (result.data) {
					plotData = result.data.marketData;
					await applyAction(result);
				}
			}
		};
	};

	onMount(() => {
		priceContext = priceChartContainer.getContext('2d');
		marketCapContext = marketCapChartContainer.getContext('2d');
		totalVolumeContext = totalVolumeChartContainer.getContext('2d');

		if (priceContext === null || marketCapContext === null || totalVolumeContext === null) {
			throw new Error('Could not get the context of the canvas element');
		}

		// Create a new configuration object for each chart
		const priceChartConfig = { ...chartConfig };
		priceChartConfig.data = { datasets: [] };
		priceChart = new Chart(priceContext, priceChartConfig);

		const marketCapChartConfig = { ...chartConfig };
		marketCapChartConfig.data = { datasets: [] };
		marketCapChart = new Chart(marketCapContext, marketCapChartConfig);

		const totalVolumeChartConfig = { ...chartConfig };
		totalVolumeChartConfig.data = { datasets: [] };
		totalVolumeChart = new Chart(totalVolumeContext, totalVolumeChartConfig);

		rendered = true;

		// Add event listeners for zooming
		priceChartContainer.addEventListener('wheel', (event) => handleZoom(event, priceChart));
		marketCapChartContainer.addEventListener('wheel', (event) => handleZoom(event, marketCapChart));
		totalVolumeChartContainer.addEventListener('wheel', (event) =>
			handleZoom(event, totalVolumeChart)
		);
	});

	/**
	 * Update the chart with new data
	 * @param {Chart<"line", { x: Date; y: number; }[], unknown>} chart - The chart to update
	 * @param {Array<Array<number>>} data - The new data to update the chart with
	 * @param {string} label - The label to use for the dataset
	 * @param {string} cryptoName - The name of the cryptocurrency
	 */
	const updateChart = (chart, data, label, cryptoName) => {
		const dataset = {
			label: `${cryptoName} ${label}`,
			data: data.map(
				/** @param {Array<number>} item */
				(item) => {
					return {
						x: new Date(item[0]),
						y: item[1]
					};
				}
			),
			fill: false,
			borderColor: '#' + Math.floor(Math.random() * 16777215).toString(16),
			tension: 0.1
		};

		chart.data.datasets.push(dataset);
		chart.update();
	};

	$: if (rendered) {
		// Clear the datasets for each chart
		priceChart.data.datasets = [];
		marketCapChart.data.datasets = [];
		totalVolumeChart.data.datasets = [];

		Object.keys(plotData).forEach(
			/** @param {string} cryptoName */
			(cryptoName) => {
				// Update each chart with the new data
				updateChart(priceChart, plotData[cryptoName].prices, 'Price', cryptoName);
				updateChart(marketCapChart, plotData[cryptoName].market_caps, 'Market Cap', cryptoName);
				updateChart(
					totalVolumeChart,
					plotData[cryptoName].total_volumes,
					'Total Volume',
					cryptoName
				);
			}
		);
	}
</script>

<form action="?/getCoinData" method="POST" use:enhance={handleCoinDataFetch}>
	<ShowError {form} />
	<div style="display: flex; justify-content: space-between;">
		<div style="flex: 2; margin-right: 10px;">
			<TagCoin
				label="Cryptocurrencies"
				id="tag-input"
				name="tags"
				value=""
				{coins}
				placeholder="Select cryptocurrencies..."
			/>
		</div>
		<div style="flex: 1; margin-left: 10px;">
			<label for="days" class="block text-[#efefef] text-sm font-bold mb-2">Days</label>
			<input
				type="number"
				id="days"
				name="days"
				value="7"
				required
				class="w-full p-4 bg-[#0a0a0a] text-[#efefef] border border-[#145369] rounded focus:outline-none focus:border-[#2596be] text-gray-500"
				placeholder="Enter days"
			/>
		</div>
	</div>
	{#if fetching}
		<Loader width={20} message="Fetching data..." />
	{:else}
		<button
			class="px-6 py-2 bg-[#041014] border border-[#145369] hover:border-[#2596be] text-[#efefef] hover:text-white rounded"
		>
			Fetch Coin Data
		</button>
	{/if}
</form>

<div in:fly={{ x: 100, duration: 1000, delay: 1000 }} out:fly={{ duration: 1000 }}>
	<canvas bind:this={priceChartContainer} />
	<canvas bind:this={marketCapChartContainer} />
	<canvas bind:this={totalVolumeChartContainer} />
</div>
```

We employed [Charts.js][83] as the charting library. It's largely simple to use. Though the component looks big, it's very straightforward. We used `JSDocs` instead of TypeScript for annotations. At first, when the DOM was mounted, we created charts with empty datasets. We then expect users to select their preferred coins and number of days. Clicking the `Fetch Coin Data` button will send the inputted data to the backend using SvelteKit's form actions. The data returned by this API call will be used to populate the plots using Svelte's reactive block dynamically. The code for the form action and the preliminary data retrieval from the backend is in `frontend/src/routes/+page.server.js`:

```js
import { BASE_API_URI } from "$lib/utils/constants";
import { fail } from "@sveltejs/kit";

/** @type {import('./$types').PageServerLoad} */
export async function load({ fetch }) {
  const fetchQuestions = async () => {
    const res = await fetch(`${BASE_API_URI}/qa/questions`);
    return res.ok && (await res.json());
  };

  const fetchCoins = async () => {
    const res = await fetch(`${BASE_API_URI}/crypto/coins`);
    return res.ok && (await res.json());
  };

  const questions = await fetchQuestions();
  const coins = await fetchCoins();

  return {
    questions,
    coins,
  };
}

// Get coin data form action

/** @type {import('./$types').Actions} */
export const actions = {
  /**
   * Get coin market history data from the API
   * @param request - The request object
   * @param fetch - Fetch object from sveltekit
   * @returns Error data or redirects user to the home page or the previous page
   */
  getCoinData: async ({ request, fetch }) => {
    const data = await request.formData();
    const coinIDs = String(data.get("tags"));
    const days = Number(data.get("days"));
    const res = await fetch(
      `${BASE_API_URI}/crypto/coin_prices?tags=${coinIDs}&currency=USD&days=${days}`
    );
    if (!res.ok) {
      const response = await res.json();
      const errors = [{ id: 1, message: response.message }];
      return fail(400, { errors: errors });
    }

    const response = await res.json();

    return {
      status: 200,
      marketData: response,
    };
  },
};
```

The endpoint used here, `${BASE_API_URI}/crypto/coin_prices?tags=${coinIDs}&currency=USD&days=${days}`, was just created and the code is:

```rust
// backend/src/routes/crypto/prices.rs

use crate::{
    settings,
    utils::{CustomAppError, CustomAppJson},
};
use axum::extract::Query;
use std::collections::HashMap;

#[derive(serde::Deserialize, Debug)]
pub struct CoinMarketDataRequest {
    tags: String,
    currency: String,
    days: i32,
}

#[derive(serde::Deserialize, Debug, serde::Serialize)]
pub struct CoinMarketData {
    prices: Vec<Vec<f64>>,
    market_caps: Vec<Vec<f64>>,
    total_volumes: Vec<Vec<f64>>,
}

#[axum::debug_handler]
#[tracing::instrument(name = "get_coin_market_data")]
pub async fn get_coin_market_data(
    Query(coin_req): Query<CoinMarketDataRequest>,
) -> Result<CustomAppJson<HashMap<String, CoinMarketData>>, CustomAppError> {
    let tag_ids: Vec<String> = coin_req.tags.split(',').map(|s| s.to_string()).collect();
    let mut responses = HashMap::new();
    let settings = settings::get_settings().expect("Failed to get settings");
    for tag_id in tag_ids {
        let url = format!(
            "{}/coins/{}/market_chart?vs_currency={}&days={}",
            settings.coingecko.api_url, &tag_id, coin_req.currency, coin_req.days
        );
        match reqwest::get(&url).await {
            Ok(response) => match response.json::<CoinMarketData>().await {
                Ok(data) => {
                    responses.insert(tag_id, data);
                }
                Err(e) => {
                    tracing::error!("Failed to parse market data from response: {}", e);
                }
            },
            Err(e) => {
                tracing::error!("Failed to fetch market data from CoinGecko: {}", e);
            }
        }
    }

    Ok(CustomAppJson(responses))
}
```

It simply uses CoinGecko's API to retrieve the history data of the coins since `days` ago. Back to the frontend code, SvelteKit version 2 made some changes that mandate [explicitly awaiting asynchronous functions in `load`][84]. This and other changes will be pointed out as the series progresses. Our `load` fetches both the questions and coins from the backend. No pagination is implemented here but it's easy to implement with `sqlx`. Pagination can also be done easily with sveltekit. You can take that up as a challenge.

The `Charts.svelte` components used some custom input components. This is simply for modularity's sake and is just simple HTML elements with tailwind CSS. Also, it used `chartConfig` and `handleZoom`. The former is just a simple configuration for the entire charts while the latter just allows simple zoom in and out of the plots. For better zooming and panning features, it's recommended to use the [chartjs-plugin-zoom][85].

With all these in place, the landing page should look like this:

![Application's home page](home-page.jpeg "Application's home page")

### Step 2: Question Detail page

The middle column on the landing page shows all the questions in the database. We need a page that zooms in on each question so that other users can provide answers. We have such a page in `frontend/src/routes/questions/[id]/+page.svelte`:

```html
<script>
	import { applyAction, enhance } from '$app/forms';
	import { page } from '$app/stores';
	import Logo from '$lib/assets/logo.png';
	import {
		formatCoinName,
		formatPrice,
		getCoinsPricesServer,
		highlightCodeBlocks,
		timeAgo
	} from '$lib/utils/helpers.js';
	import { afterUpdate, onMount } from 'svelte';
	import Loader from '$lib/components/Loader.svelte';
	import { scale } from 'svelte/transition';
	import { flip } from 'svelte/animate';
	import Modal from '$lib/components/Modal.svelte';
	import hljs from 'highlight.js';
	import ShowError from '$lib/components/ShowError.svelte';
	import { notification } from '$lib/stores/notification.store.js';
	import 'highlight.js/styles/night-owl.css';
	import TextArea from '$lib/components/inputs/TextArea.svelte';

	export let data;

	/** @type {import('./$types').ActionData} */
	export let form;
	/** @type {Array<{"name": String, "price": number}>} */
	let coinPrices = [],
		processing = false,
		showDeleteModal = false,
		showEditModal = false,
		answerID = '',
		answerContent = '';

	$: ({ question, answers } = data);

	const openModal = (isDelete = true) => {
		if (isDelete) {
			showDeleteModal = true;
		} else {
			showEditModal = true;
		}
	};

	const closeModal = () => {
		showDeleteModal = false;
		showEditModal = false;
	};

	/** @param {String} id */
	const setAnswerID = (id) => (answerID = id);
	/** @param {String} content */
	const setAnswerContent = (content) => (answerContent = content);

	onMount(async () => {
		highlightCodeBlocks(hljs);
		if (question) {
			const tagsString = question.tags
				.map(
					/** @param {{"id": String}} tag */
					(tag) => tag.id
				)
				.join(',');
			coinPrices = await getCoinsPricesServer($page.data.fetch, tagsString, 'usd');
		}
	});

	afterUpdate(() => {
		highlightCodeBlocks(hljs);
	});

	/** @type {import('./$types').SubmitFunction} */
	const handleAnswerQuestion = async () => {
		processing = true;
		return async ({ result }) => {
			processing = false;
			if (result.type === 'success') {
				if (result.data && 'answer' in result.data) {
					answers = [result.data.answer, ...answers];
					answerContent = '';
					notification.set({ message: 'Answer posted successfully', colorName: 'blue' });
				}
			}
			await applyAction(result);
		};
	};

	/** @type {import('./$types').SubmitFunction} */
	const handleDeleteAnswer = async () => {
		return async ({ result }) => {
			closeModal();
			if (result.type === 'success') {
				answers = answers.filter(
					/** @param {{"id": String}} answer */
					(answer) => answer.id !== answerID
				);
				notification.set({ message: 'Answer deleted successfully', colorName: 'blue' });
			}
			await applyAction(result);
		};
	};

	/** @type {import('./$types').SubmitFunction} */
	const handleUpdateAnswer = async () => {
		return async ({ result }) => {
			closeModal();
			if (result.type === 'success') {
				answers = answers.map(
					/** @param {{"id": String}} answer */
					(answer) => {
						if (result.data && 'answer' in result.data) {
							return answer.id === answerID ? result.data.answer : answer;
						}
						return answer;
					}
				);
				answerContent = '';
				notification.set({ message: 'Answer updated successfully', colorName: 'blue' });
			}
			await applyAction(result);
		};
	};
</script>

<div class="max-w-5xl mx-auto p-4">
	<!-- Stats Section -->
	<div class="bg-[#0a0a0a] p-6 rounded-lg shadow mb-6 flex justify-between items-center">
		<p>Asked: {timeAgo(question.created_at)}</p>
		<p>Modified: {timeAgo(question.updated_at)}</p>
	</div>
	<div class="grid grid-cols-1 md:grid-cols-12 gap-4">
		<!-- Main Content -->
		<div class="md:col-span-9">
			<!-- Question Section -->
			<div class="bg-[#041014] p-6 rounded-lg shadow mb-6 border border-black">
				<h1 class="text-2xl font-bold mb-4">{question.title}</h1>
				<p>{@html question.content}</p>
				<div class="flex mt-4 flex-wrap">
					{#each question.tags as tag}
						<span
							class="mr-2 mb-2 px-3 py-1 text-sm bg-[#041014] border border-[#145369] hover:border-[#2596be] rounded"
						>
							{tag.name.toLowerCase()}
						</span>
					{/each}
				</div>
				<div class="flex justify-end mt-4">
					{#if $page.data.user && question.author.id === $page.data.user.id}
						<a
							class="mr-2 text-blue-500 hover:text-blue-600"
							href="/questions/{question.id}/update"
						>
							Edit
						</a>
						<a class="mr-2 text-red-500 hover:text-red-600" href="/questions/{question.id}/delete">
							Delete
						</a>
					{/if}
				</div>
				<hr class="my-4" />
				<div class="flex justify-end items-center">
					<span class="mr-3">
						{question.author.first_name + ' ' + question.author.last_name}
					</span>
					<img
						src={question.author.thumbnail ? question.author.thumbnail : Logo}
						alt={question.author.first_name + ' ' + question.author.last_name}
						class="h-10 w-10 rounded-full"
					/>
				</div>
			</div>

			<!-- Answers Section -->
			<h2 class="text-xl font-bold mb-4">Answers</h2>
			{#each answers as answer (answer.id)}
				<div
					class="bg-[#041014] p-6 rounded-lg shadow mb-4"
					transition:scale|local={{ start: 0.4 }}
					animate:flip={{ duration: 200 }}
				>
					<p>{@html answer.content}</p>

					<div class="flex justify-end mt-4">
						{#if $page.data.user && answer.author.id === $page.data.user.id}
							<button
								class="mr-2 text-blue-500 hover:text-blue-600"
								on:click={() => {
									openModal(false);
									setAnswerID(answer.id);
									setAnswerContent(answer.raw_content);
								}}
							>
								Edit
							</button>
							<button
								class="mr-2 text-red-500 hover:text-red-600"
								on:click={() => {
									openModal();
									setAnswerID(answer.id);
								}}
							>
								Delete
							</button>
						{/if}
					</div>
					<hr class="my-4" />
					<div class="flex justify-end items-center">
						<span class="mr-3">{answer.author.first_name + ' ' + answer.author.last_name}</span>
						<img
							src={answer.author.thumbnail ? answer.author.thumbnail : Logo}
							alt={answer.author.first_name + ' ' + answer.author.last_name}
							class="h-10 w-10 rounded-full"
						/>
					</div>
				</div>
			{:else}
				<div class="bg-[#041014] p-6 rounded-lg shadow mb-4">
					<p>No answers yet.</p>
				</div>
			{/each}

			<!-- Post Answer Section -->
			<form
				class="bg-[#041014] p-6 rounded-lg shadow"
				method="POST"
				action="?/answer"
				use:enhance={handleAnswerQuestion}
			>
				<h2 class="text-xl font-bold mb-4">Your Answer</h2>
				<ShowError {form} />

				<TextArea
					label=""
					id="answer"
					name="content"
					placeholder="Write your answer here (markdown supported)..."
					bind:value={answerContent}
				/>

				{#if processing}
					<Loader width={20} message="Posting your answer..." />
				{:else}
					<button
						class="mt-4 px-6 py-2 bg-[#041014] border border-[#145369] hover:border-[#2596be] text-white rounded"
					>
						{#if $page.data.user && $page.data.user.id === question.author.id}
							Answer your question
						{:else}
							Post Your Answer
						{/if}
					</button>
				{/if}
			</form>
		</div>

		<!-- Right Sidebar -->
		<div class="md:col-span-3">
			<h2 class="text-xl font-semibold mb-4">Current prices</h2>
			<div
				class="bg-[#041014] rounded-lg shadow p-4 hover:bg-black border border-black hover:border-[#145369]"
			>
				<div class="space-y-4">
					{#each coinPrices as coin (coin.name)}
						<div
							class="bg-[#145369] p-3 rounded-lg text-center"
							transition:scale|local={{ start: 0.4 }}
							animate:flip={{ duration: 200 }}
						>
							<p class="text-3xl font-bold">
								<span class="text-base">$</span>{formatPrice(coin.price)}
							</p>
							{#if question.tags.find(/** @param {{"id": String}} tag */ (tag) => tag.id === coin.name)}
								<div class="flex items-center text-lg">
									<img
										class="w-8 h-8 rounded-full mr-2 transition-transform duration-500 ease-in-out transform hover:rotate-180"
										src={question.tags.find(
											/** @param {{"id": String}} tag */
											(tag) => tag.id === coin.name
										).image}
										alt={coin.name}
									/>
									<span class="mr-2">
										{formatCoinName(
											coin.name,
											question.tags.find(
												/** @param {{"id": String}} tag */
												(tag) => tag.id === coin.name
											).symbol
										)}
									</span>
								</div>
							{/if}
						</div>
					{/each}
				</div>
			</div>
		</div>
	</div>
</div>

{#if showDeleteModal}
	<Modal on:close={closeModal}>
		<form
			class="bg-[#041014] p-6 rounded-lg shadow text-center"
			method="POST"
			action="?/deleteAnswer"
			use:enhance={handleDeleteAnswer}
		>
			<ShowError {form} />
			<p class="text-red-500 p-3 mb-4 italic">
				Are you sure you want to delete this answer (id={answerID})
			</p>
			<input type="hidden" name="answerID" value={answerID} />
			<button
				class="mt-4 px-6 py-2 bg-[#041014] border border-red-400 hover:border-red-700 text-red-600 rounded"
			>
				Delete Answer
			</button>
		</form>
	</Modal>
{/if}

{#if showEditModal}
	<Modal on:close={closeModal}>
		<form
			class="bg-[#041014] p-6 rounded-lg shadow text-center"
			method="POST"
			action="?/updateAnswer"
			use:enhance={handleUpdateAnswer}
		>
			<ShowError {form} />
			<input type="hidden" name="answerID" value={answerID} />
			<textarea
				class="w-full p-4 bg-[#0a0a0a] text-[#efefef] border border-[#145369] rounded focus:border-[#2596be] focus:outline-none"
				rows="6"
				bind:value={answerContent}
				name="content"
				placeholder="Write your answer here (markdown supported)..."
			/>
			<button
				class="mt-4 px-6 py-2 bg-[#041014] border border-[#145369] hover:border-[#2596be] text-white rounded"
			>
				Update Answer
			</button>
		</form>
	</Modal>
{/if}
```

It has two columns:

1. The first shows the question and all the answers to that question.
2. The second shows the current price of the coin tagged in the question. The prices do not get updated live or in real time, you need to refresh the page for updated prices but this can be improved using web sockets.

This page has an accompanying `+page.server.js` that fetches the data the page uses and handles other subsequent interactions such as posting, updating, and deleting answers:

```js
import { BASE_API_URI } from "$lib/utils/constants";
import { fail } from "@sveltejs/kit";

/** @type {import('./$types').PageServerLoad} */
export async function load({ fetch, params }) {
  const fetchQuestion = async () => {
    const res = await fetch(`${BASE_API_URI}/qa/questions/${params.id}`);
    return res.ok && (await res.json());
  };

  const fetchAnswers = async () => {
    const res = await fetch(
      `${BASE_API_URI}/qa/questions/${params.id}/answers`
    );
    return res.ok && (await res.json());
  };

  return {
    question: await fetchQuestion(),
    answers: await fetchAnswers(),
  };
}

/** @type {import('./$types').Actions} */
export const actions = {
  /**
   *
   * @param request - The request object
   * @param fetch - Fetch object from sveltekit
   * @returns Error data or redirects user to the home page or the previous page
   */
  answer: async ({ request, fetch, params, cookies }) => {
    const data = await request.formData();
    const content = String(data.get("content"));

    /** @type {RequestInit} */
    const requestInitOptions = {
      method: "POST",
      credentials: "include",
      headers: {
        "Content-Type": "application/json",
        Cookie: `sessionid=${cookies.get("cryptoflow-sessionid")}`,
      },
      body: JSON.stringify({
        content: content,
      }),
    };

    const res = await fetch(
      `${BASE_API_URI}/qa/answer/${params.id}`,
      requestInitOptions
    );

    if (!res.ok) {
      const response = await res.json();
      const errors = [{ id: 1, message: response.message }];
      return fail(400, { errors: errors });
    }

    const response = await res.json();

    return {
      status: 200,
      answer: response,
    };
  },
  /**
   *
   * @param request - The request object
   * @param fetch - Fetch object from sveltekit
   * @returns Error data or redirects user to the home page or the previous page
   */
  deleteAnswer: async ({ request, fetch, cookies }) => {
    const data = await request.formData();
    const answerID = String(data.get("answerID"));

    /** @type {RequestInit} */
    const requestInitOptions = {
      method: "DELETE",
      credentials: "include",
      headers: {
        "Content-Type": "application/json",
        Cookie: `sessionid=${cookies.get("cryptoflow-sessionid")}`,
      },
    };

    const res = await fetch(
      `${BASE_API_URI}/qa/answers/${answerID}`,
      requestInitOptions
    );

    if (!res.ok) {
      const response = await res.json();
      const errors = [{ id: 1, message: response.message }];
      return fail(400, { errors: errors });
    }

    return {
      status: res.status,
    };
  },
  /**
   *
   * @param request - The request object
   * @param fetch - Fetch object from sveltekit
   * @returns Error data or redirects user to the home page or the previous page
   */
  updateAnswer: async ({ request, fetch, cookies }) => {
    const data = await request.formData();
    const answerID = String(data.get("answerID"));
    const content = String(data.get("content"));

    /** @type {RequestInit} */
    const requestInitOptions = {
      method: "PATCH",
      credentials: "include",
      headers: {
        "Content-Type": "application/json",
        Cookie: `sessionid=${cookies.get("cryptoflow-sessionid")}`,
      },
      body: JSON.stringify({
        content: content,
      }),
    };

    const res = await fetch(
      `${BASE_API_URI}/qa/answers/${answerID}`,
      requestInitOptions
    );

    if (!res.ok) {
      const response = await res.json();
      const errors = [{ id: 1, message: response.message }];
      return fail(400, { errors: errors });
    }

    return {
      status: res.status,
      answer: await res.json(),
    };
  },
};
```

It's just the familiar structure with a `load` function and a bunch of other form actions. Since all the other pages have this structure, I will skip explaining them but will include their screenshots

You can follow along by reading through the code on GitHub. They are very easy to follow.

The question detail page looks like this:

![Question detailed page](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/fl2qbcb85lll1uhnw7jb.jpeg "Question detailed page")

As for login and signup pages, we have these:

![Login page](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/41wxzvvmu9mtewnpakwp.jpeg "Login page")

![Signup page](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/yug58aawe2jr58qw2r4r.jpeg "Signup page")

When one registers, a one-time token is sent to the user's email. There's a page to input this token and get the account attached to it activated. The page looks like this:

![Activate account page](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/h6d9mc1utd15s05yw40m.jpeg "Activate account page")

With that, we end this series. Kindly check the series' GitHub repository for the updated and complete code. They are intuitive.

I apologize once again for the abandonment.

## Outro

Enjoyed this article? I'm a Software Engineer and Technical Writer actively seeking new opportunities, particularly in areas related to web security, finance, health care, and education. If you think my expertise aligns with your team's needs, let's chat! You can find me on LinkedIn: [LinkedIn](https://www.linkedin.com/in/idogun-john-nelson/) and Twitter: [Twitter](https://twitter.com/Sirneij).

If you found this article valuable, consider sharing it with your network to help spread the knowledge!

[81]: https://cryptoflow-one.vercel.app/ "CryptoFlow live application"
[82]: https://docs.render.com/free "Deploy for Free"
[83]: https://www.chartjs.org/docs/latest/ "Chart.js"
[84]: https://kit.svelte.dev/docs/migrating-to-sveltekit-2#top-level-promises-are-no-longer-awaited "Top-level promises are no longer awaited"
[85]: https://www.chartjs.org/chartjs-plugin-zoom/latest/guide/ "A zoom and pan plugin for Chart.js >= 3.0.0"
[100]: https://github.com/Sirneij/cryptoflow "A Q&A web application to demostrate how to build a secured and scalable client-server application with axum and sveltekit"
