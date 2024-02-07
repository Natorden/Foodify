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
    recipeId uuid NOT NULL
     CONSTRAINT FK__recipe_steps__recipes REFERENCES recipes,
    priority int NOT NULL,
    title text NOT NULL,
    description TEXT NOT NULL,
    PRIMARY KEY (recipeId,priority)
)
