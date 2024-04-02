CREATE DATABASE foodify_comments;

\c "foodify_comments"

CREATE TABLE comments(
    ID uuid DEFAULT gen_random_uuid() PRIMARY KEY,
    user_id UUID NOT NULL,
    recipe_id UUID NOT NULL,
    content TEXT NOT NULL,
    created_at timestamptz DEFAULT now() NOT NULL
);

CREATE TABLE comment_likes(
    comment_id UUID NOT NULL
        CONSTRAINT FK__comment_likes__comments REFERENCES comments,
    user_id UUID NOT NULL,
    PRIMARY KEY (comment_id, user_id)
)