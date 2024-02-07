CREATE TABLE IF NOT EXISTS users (
    id UUID,
    name VARCHAR(255) NOT NULL,
    max_players_in_list INTEGER NOT NULL DEFAULT 100,
    max_lists_count INTEGER NOT NULL DEFAULT 30,
    
    CONSTRAINT pk_users PRIMARY KEY (id),
    CONSTRAINT uq_users_names UNIQUE (name),
    CONSTRAINT ch_max_players_val_range CHECK (max_players_in_list >= 5 AND max_players_in_list <= 500),
    CONSTRAINT ch_max_lists_val_range CHECK (max_lists_count >= 5 AND max_lists_count <= 100)
);

CREATE TABLE IF NOT EXISTS players_lists (
    id UUID,
    name VARCHAR(255) NOT NULL,
    user_id UUID NOT NULL,
    
    CONSTRAINT pk_players_lists PRIMARY KEY (id),
    CONSTRAINT ch_name_len_range CHECK (LENGTH(name) >= 1),
    CONSTRAINT uq_names_per_user UNIQUE (name, user_id),
    CONSTRAINT fk_player_lists_by_user FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS organises (
    club_id UUID,
    user_id UUID NOT NULL,
    
    CONSTRAINT pk_organises PRIMARY KEY (club_id),
    CONSTRAINT fk_organises_by_user FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS contains (
    list_id UUID,
    player_id UUID,
    
    CONSTRAINT pk_contains PRIMARY KEY (list_id, player_id),
    CONSTRAINT fk_contains_by_lists FOREIGN KEY (list_id) REFERENCES players_lists(id) ON DELETE CASCADE
);
