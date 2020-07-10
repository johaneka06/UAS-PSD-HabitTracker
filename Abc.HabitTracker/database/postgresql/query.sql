--Database: Postgresql 12

CREATE TABLE badge
(
	badge_id	UUID,
	user_id		UUID,
	badge_name	VARCHAR(255) NOT NULL,
	description	VARCHAR(255) NOT NULL,
	created_at	TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
	UNIQUE(badge_id),
	PRIMARY KEY(badge_id, user_id)
);

CREATE TABLE habit
(
	habit_id	UUID PRIMARY KEY,
	habit_name	VARCHAR(255) NOT NULL,
	days_off	TEXT[],
	user_id		UUID NOT NULL,
	created_at	TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
	updated_at	TIMESTAMPTZ,
	deleted_at	TIMESTAMPTZ
);

CREATE TABLE habit_log
(
	log_id			UUID PRIMARY KEY,
	habit_id		UUID NOT NULL,
	user_id			UUID NOT NULL,
	log_date		TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
	current_streak	INT NOT NULL,
	longest_streak	INT NOT NULL,
	FOREIGN KEY (habit_id) REFERENCES habit(habit_id)
);

CREATE TABLE log_snapshot
(
	snapshot_id			UUID PRIMARY KEY,
	last_snapshot_id	UUID NOT NULL,
	last_habit_id		UUID NOT NULL,
	user_id				UUID NOT NULL,
	log_count			INT NOT NULL,
	longest_streak		INT NOT NULL,
	current_streak		INT NOT NULL,
	last_snapshot_time	TIMESTAMPTZ,
	current_snapshot_at	TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
	FOREIGN KEY (last_habit_id) REFERENCES habit(habit_id)
);