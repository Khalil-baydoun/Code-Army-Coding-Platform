import { format } from "date-fns";
import { observer } from "mobx-react-lite";
import React, { useContext } from "react";
import { Field, Form as FinalForm } from "react-final-form";
import { Link } from "react-router-dom";
import { Button, Comment, Form, Header } from "semantic-ui-react";
import TextAreaInput from "../../app/common/form/TextAreaInput";
import { RootStoreContext } from "../../app/stores/rootStore";

const CommentForm = () => {
  const rootStore = useContext(RootStoreContext);
  const { addComment, problem } = rootStore.problemStore;

  return (
    <Comment.Group>
      <Header as="h3" dividing>
        Comments
      </Header>
      {problem &&
        problem.Comments &&
        problem.Comments.map((comment) => (
          <Comment key={comment.Id}>
            <Comment.Content>
              <Comment.Author as={Link} to={`/profile/${comment.AuthorEmail}`}>
                {comment.AuthorName}
              </Comment.Author>
              <Comment.Metadata>
                <div>
                  {format(comment.CreatedAt!, "eeee do MMMM")} at{" "}
                  {format(comment.CreatedAt!, "h:mm a")}
                </div>
              </Comment.Metadata>
              <Comment.Text>{comment.Body}</Comment.Text>
            </Comment.Content>
          </Comment>
        ))}
      <FinalForm
        onSubmit={addComment}
        render={({ handleSubmit, submitting, form }) => (
          <Form onSubmit={() => handleSubmit()!.then(() => form.reset())}>
            <Field
              name="Body"
              component={TextAreaInput}
              rows={2}
              placeholder="Add your comment"
            />
            <Button
              content="Add Reply"
              labelPosition="left"
              icon="edit"
              primary
              color="teal"
              loading={submitting}
            />
          </Form>
        )}
      />
    </Comment.Group>
  );
};

export default observer(CommentForm);
