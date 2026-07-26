# FitnessTracker

A workout tracking app built with ASP.NET Core 8, PostgreSQL, and Vue 3. Users can log workouts, add exercises, and track sets with reps and weight.

## Stack

- Backend: ASP.NET Core 8, Entity Framework Core
- Database: PostgreSQL
- Auth: ASP.NET Identity, JWT + refresh token rotation
- Frontend: Vue 3
- Containerized: Docker + Docker Compose

## Running the project

The easiest way is with Docker — no local .NET or PostgreSQL needed.

### 1. Clone the repo

```bash
git clone https://github.com/eg7001/FitnessTrackerFin
cd fitness-tracker
```

### 2. Create a `.env` file in the root

Copy `.env.example` to `.env` and fill in real values:

```env
POSTGRES_PASSWORD=yourpassword
JWT_KEY=your-secret-key-at-least-32-chars
```

(Postgres database/user, JWT issuer/audience/expiry, and the CORS-allowed
origin are already set in `docker-compose.yml` — only the two secrets above
need to be supplied.)

### 3. Start everything

```bash
docker compose up --build
```

- Frontend → `http://localhost` (proxies `/api/*` to the backend)
- Backend API → `http://localhost:8080`

Scalar's interactive API docs (`/scalar/`) are only enabled in the
Development environment, so they're not exposed by the Docker Compose
setup. To use them, run the backend locally instead: `dotnet run` from
`backend/`, then visit `https://localhost:7008/scalar/`.

### 4. Run migrations

```bash
docker compose exec backend dotnet ef database update
```

---

## API Overview

All routes except auth require a Bearer token in the `Authorization` header.

### Auth

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register |
| POST | `/api/auth/login` | Login — returns access + refresh token |
| POST | `/api/auth/refresh` | Rotate refresh token |

### Workouts

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/workouts` | Get your workouts (paginated) |
| GET | `/api/workouts/{id}` | Get a single workout with exercises and sets |
| POST | `/api/workouts` | Create a workout |
| PUT | `/api/workouts/{id}` | Update name/date |
| DELETE | `/api/workouts/{id}` | Delete a workout |

### Exercises

Global exercises any user can browse. Create/edit/delete currently requires auth — admin restriction planned.

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/exercises` | List exercises (filter by name, muscle group) |
| GET | `/api/exercises/{id}` | Get by ID |
| POST | `/api/exercises` | Create |
| PUT | `/api/exercises/{id}` | Update |
| DELETE | `/api/exercises/{id}` | Delete |

### Workout Exercises

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/workouts/{workoutId}/exercises` | Add exercise to workout |
| DELETE | `/api/workouts/{workoutId}/exercises/{workoutExerciseId}` | Remove it |

### Sets

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/workout-exercises/{workoutExerciseId}/sets` | Add a set |
| PUT | `/api/sets/{id}` | Update a set |
| DELETE | `/api/sets/{id}` | Delete a set |

## Data model

```
User → Workouts → WorkoutExercises ←→ Exercises (global)
                       └── Sets
```
