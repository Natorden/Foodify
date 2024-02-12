CREATE DATABASE foodify_recipe;

\c "foodify_recipe"

CREATE TABLE tags(
    ID uuid DEFAULT gen_random_uuid() PRIMARY KEY,
    name TEXT NOT NULL,
    description TEXT NOT NULL
);

CREATE TABLE ingredients(
    ID uuid DEFAULT gen_random_uuid() PRIMARY KEY,
    name TEXT NOT NULL
);

CREATE TABLE recipes(
    ID uuid NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    title TEXT NOT NULL,
    info TEXT NOT NULL,
    created_by_id uuid NOT NULL
);

CREATE TABLE recipe_steps(
    recipe_id uuid NOT NULL
     CONSTRAINT FK__recipe_steps__recipes REFERENCES recipes,
    priority INT NOT NULL,
    title TEXT NOT NULL,
    description TEXT NOT NULL,
    PRIMARY KEY (recipe_id,priority)
);

CREATE TABLE recipe_tags(
    recipe_id UUID NOT NULL
        CONSTRAINT FK__recipe_tags__recipes REFERENCES recipes,
    tag_id UUID NOT NULL
        CONSTRAINT FK__recipe_tags__tags REFERENCES tags,
        PRIMARY KEY (recipe_id,tag_id)
);

