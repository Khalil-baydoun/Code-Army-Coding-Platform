import { observer } from "mobx-react-lite";
import React, { Fragment, useContext, useEffect } from "react";
import {
  Route,
  RouteComponentProps,
  Switch,
  withRouter,
} from "react-router-dom";
import { ToastContainer } from "react-toastify";
import { Container } from "semantic-ui-react";
import CourseDashboard from "../../features/dashboard/Course/CourseDashboard";
import CourseManagementDashboard from "../../features/dashboard/Course/CourseManagementDashboard";
import ProblemSetList from "../../features/dashboard/ProblemSet/ProblemSetList";
import ProblemSetManagementDashboard from "../../features/dashboard/ProblemSet/ProblemSetManagementDashboard";
import ProblemSetProblemsList from "../../features/dashboard/ProblemSet/ProblemSetProblemsList";
import CreateCourseForm from "../../features/forms/CreateCourseForm";
import CreateProblemForm from "../../features/forms/CreateProblemForm";
import CreateProblemSetForm from "../../features/forms/CreateProblemSetForm";
import SubmitForm from "../../features/forms/SubmitForm";
import HomePage from "../../features/Home/HomePage";
import NavBar from "../../features/nav/NavBar";
import VerdictsGuide from "../../features/nav/VerdictsGuide";
import Problem from "../../features/problem/problem";
import SubmissionHistory from "../../features/submission/SubmissionHistory";
import UserStatistics from "../../features/submission/UserStatistics";
import ModalContainer from "../common/modals/ModalContainer";
import { RootStoreContext } from "../stores/rootStore";
import Forbidden from "./Forbidden";
import LoadingComponent from "./LoadingComponent";
import NotFound from "./NotFound";
import PrivateRoute from "./PrivateRoute";

const App: React.FC<RouteComponentProps> = ({ location }) => {
  const rootStore = useContext(RootStoreContext);
  const { setAppLoaded, token, appLoaded } = rootStore.commonStore;
  const { getUser, hasRole } = rootStore.userStore;

  useEffect(() => {
    if (token) {
      getUser().finally(() => setAppLoaded());
    } else {
      setAppLoaded();
    }
  }, [getUser, setAppLoaded, token]);

  if (!appLoaded) return <LoadingComponent />;

  return (
    <Fragment>
      <ToastContainer position="bottom-right" />
      <ModalContainer />
      <Route exact path="/" component={HomePage} />
      <Route
        path={"/(.+)"}
        render={() => (
          <Fragment>
            <NavBar />
            <Container style={{ marginTop: "7em" }}>
              <Switch>
                <PrivateRoute
                  exact
                  path="/course"
                  component={CourseDashboard}
                />
                <PrivateRoute
                  exact
                  path="/verdictsGuide"
                  component={VerdictsGuide}
                />
                <PrivateRoute
                  path="/submissions/page/:pageNumber"
                  component={SubmissionHistory}
                />
                <PrivateRoute
                  exact
                  path="/statistics"
                  component={UserStatistics}
                />
                <PrivateRoute
                  exact
                  path="/course/:courseId/problemSet/:problemSetId/problems/:problemId"
                  component={Problem}
                />
                <PrivateRoute
                  exact
                  path="/course/:id/problemset"
                  component={ProblemSetList}
                />
                <PrivateRoute
                  exact
                  path="/course/:courseId/problemset/:problemSetId/problems"
                  component={ProblemSetProblemsList}
                />
                <PrivateRoute
                  exact
                  path="/course/:courseId/problemset/:problemSetId/problems/:problemId/submit"
                  component={SubmitForm}
                />

                {(hasRole("Instructor") || hasRole("Admin")) && (
                  <Fragment>
                    <PrivateRoute
                      exact
                      path="/course/:courseId/manage"
                      component={CourseManagementDashboard}
                    />
                    <PrivateRoute
                      exact
                      path="/course/:courseId/problemSet/:problemSetId/manage"
                      component={ProblemSetManagementDashboard}
                    />
                    <PrivateRoute
                      exact
                      path="/course/create"
                      component={CreateCourseForm}
                    />
                    <PrivateRoute
                      exact
                      path="/course/:courseId/problemset/:problemSetId/create"
                      component={CreateProblemForm}
                    />
                    <PrivateRoute
                      exact
                      path="/course/:courseId/problemset/create"
                      component={CreateProblemSetForm}
                    />
                  </Fragment>
                )}
                <PrivateRoute path="/forbidden" component={Forbidden} />
                <Route component={NotFound} />
              </Switch>
            </Container>
          </Fragment>
        )}
      />
    </Fragment>
  );
};

export default withRouter(observer(App));
