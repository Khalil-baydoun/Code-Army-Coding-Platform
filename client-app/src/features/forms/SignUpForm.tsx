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
import { IUserSignUpValues } from "../../app/models/user";
import { RootStoreContext } from "../../app/stores/rootStore";
import LoginForm from "./LoginForm";

const validate = combineValidators({
  Email: isRequired("Email"),
  Password: isRequired("Password"),
  FirstName: isRequired("First Name"),
  LastName: isRequired("Last Name"),
});

const SignUpForm = () => {
  const rootStore = useContext(RootStoreContext);
  const { signup } = rootStore.userStore;
  const { openModal } = rootStore.modalStore;

  const getErrorMessage = (error: any): string => {
    var result = "Data is invalid";
    if (error.data.Exception.includes("Password is not valid")) {
      result =
        "The Password is not valid, it should contain a number, a capital letter, and has a minimum of 8 characters";
    } else if (error.data.Exception.includes("AUB Email")) {
      result = "The Email entered is not a valid AUB Email";
    } else if (error.data.Exception.includes("Email")) {
      result = "Email already in use!";
    }
    return result;
  };

  return (
    <Grid textAlign="center" verticalAlign="middle">
      <Grid.Column style={{ maxWidth: 450 }}>
        <Header as="h2" color="black" textAlign="center">
          Create a New Account
        </Header>
        <FinalForm
          onSubmit={(values: IUserSignUpValues) =>
            signup(values).catch((error) => ({
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
                  name="Email"
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
                  name="Password"
                />
                <Field
                  fluid
                  component={TextInput}
                  icon="user"
                  iconPosition="left"
                  placeholder="First Name"
                  name="FirstName"
                />
                <Field
                  fluid
                  component={TextInput}
                  icon="user"
                  iconPosition="left"
                  placeholder="Last Name"
                  name="LastName"
                />
                {submitError && !dirtySinceLastSubmit && !submitting && (
                  <ErrorMessage
                    error={submitError}
                    text={getErrorMessage(submitError)}
                  />
                )}
                <Button
                  disabled={(invalid && !dirtySinceLastSubmit) || pristine}
                  color="black"
                  fluid
                  size="large"
                  content="SignUp"
                  loading={submitting}
                ></Button>
              </Segment>
            </Form>
          )}
        />
        <Message>
          Already have an account?{" "}
          <a href="#" onClick={() => openModal(<LoginForm />)}>
            Sign In
          </a>
        </Message>
      </Grid.Column>
    </Grid>
  );
};
export default SignUpForm;
