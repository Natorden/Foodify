CREATE DATABASE foodify_recipe;

\c "foodify_recipe"

CREATE TABLE tags(
    ID uuid DEFAULT gen_random_uuid() PRIMARY KEY,
    name TEXT NOT NULL
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
    PRIMARY KEY (recipe_id, priority)
);

CREATE TABLE recipe_tags(
    recipe_id UUID NOT NULL
        CONSTRAINT FK__recipe_tags__recipes REFERENCES recipes,
    tag_id UUID NOT NULL
        CONSTRAINT FK__recipe_tags__tags REFERENCES tags,
    priority int NOT NULL,
    PRIMARY KEY (recipe_id, tag_id),
    CONSTRAINT UQ__recipe_tags__priority UNIQUE (recipe_id, priority)
);

CREATE TABLE recipe_ingredients(
    recipe_id UUID NOT NULL
        CONSTRAINT FK__recipe_ingredients__recipes REFERENCES recipes,
    ingredient_id UUID NOT NULL
        CONSTRAINT FK__recipe_ingredients__ingredients REFERENCES ingredients,
    unit int NOT NULL,
    amount int NOT NULL,
    priority int NOT NULL,
    PRIMARY KEY (recipe_id, ingredient_id),
    CONSTRAINT UQ__recipe_ingredients__priority UNIQUE (recipe_id, priority)
);

CREATE TABLE recipe_images(
    recipe_id uuid NOT NULL
        CONSTRAINT FK__recipe_images__recipes REFERENCES recipes,
    priority int NOT NULL,
    url text NOT NULL,
    PRIMARY KEY (recipe_id, priority)
);