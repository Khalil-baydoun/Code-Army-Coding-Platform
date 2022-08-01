import { FORM_ERROR } from "final-form";
import React, { useContext } from "react";
import { Field, Form as FinalForm } from "react-final-form";
import { combineValidators, isRequired } from "revalidate";
import {
  Button,
  Form,
  Grid,
  Header,
  Message,
  Segment,
} from "semantic-ui-react";
import ErrorMessage from "../../app/common/form/ErrorMessage";
import TextInput from "../../app/common/form/TextInput";
import { IUserFormValues } from "../../app/models/user";
import { RootStoreContext } from "../../app/stores/rootStore";
import SignUpForm from "./SignUpForm";

const validate = combineValidators({
  email: isRequired("Email"),
  password: isRequired("Password"),
});

const LoginForm = () => {
  const rootStore = useContext(RootStoreContext);
  const { login } = rootStore.userStore;
  const { openModal } = rootStore.modalStore;
  return (
    <Grid textAlign="center" verticalAlign="middle">
      <Grid.Column style={{ maxWidth: 450 }}>
        <Header as="h2" color="black" textAlign="center">
          Log-in to your account
        </Header>
        <FinalForm
          onSubmit={(values: IUserFormValues) =>
            login(values).catch((error) => ({
              [FORM_ERROR]: error,
            }))
          }
          validate={validate}
          render={({
            handleSubmit,
            submitting,
            submitError,
            invalid,
            pristine,
            dirtySinceLastSubmit,
          }) => (
            <Form size="large" onSubmit={handleSubmit} error>
              <Segment stacked>
                <Field
                  name="email"
                  component={TextInput}
                  fluid
                  icon="user"
                  iconPosition="left"
                  placeholder="E-mail address"
                />
                <Field
                  fluid
                  component={TextInput}
                  icon="lock"
                  iconPosition="left"
                  placeholder="Password"
                  type="password"
                  name="password"
                />
                {submitError && !dirtySinceLastSubmit && !submitting && (
                  <ErrorMessage
                    error={submitError}
                    text="Invalid email or password"
                  />
                )}
                <Button
                  disabled={(invalid && !dirtySinceLastSubmit) || pristine}
                  color="black"
                  fluid
                  size="large"
                  content="login"
                  loading={submitting}
                ></Button>
              </Segment>
            </Form>
          )}
        />
        <Message>
          New to us?{" "}
          <a href="#" onClick={() => openModal(<SignUpForm />)}>
            Sign Up
          </a>
        </Message>
      </Grid.Column>
    </Grid>
  );
};
export default LoginForm;
