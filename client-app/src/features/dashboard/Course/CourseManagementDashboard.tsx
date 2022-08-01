import { observer } from "mobx-react-lite";
import React, { useContext, useEffect } from "react";
import { RouteComponentProps } from "react-router";
import { Segment, Tab } from "semantic-ui-react";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import { RootStoreContext } from "../../../app/stores/rootStore";
import EditCourseForm from "../../forms/EditCourseForm";
import CourseStatistics from "./CourseStatistics";
import CourseUsersList from "./CourseUsersList";

interface DetailParams {
  courseId: string;
}

const CourseManagementDashboard: React.FC<
  RouteComponentProps<DetailParams>
> = ({ match }) => {
  const rootStore = useContext(RootStoreContext);
  const {
    getCourse,
    course,
    loadingInitial,
    getCourseStatistics,
  } = rootStore.courseProblemSetStore;
  useEffect(() => {
    getCourse(match.params.courseId);
    getCourseStatistics(match.params.courseId);
  }, [match.params.courseId, getCourse, getCourseStatistics]);

  if (loadingInitial) return <LoadingComponent />;

  if (!course) {
    return <h2>course not found!</h2>;
  }
  const panes = [
    {
      menuItem: {
        key: "edit",
        content: "Course Edit",
        icon: "edit",
      },
      render: () => (
        <Tab.Pane>
          <EditCourseForm />
        </Tab.Pane>
      ),
    },
    {
      menuItem: {
        key: "statistics",
        content: "Statistics",
        icon: "pie chart",
      },
      render: () => (
        <Tab.Pane>
          <CourseStatistics />
        </Tab.Pane>
      ),
    },
    // {
    //   menuItem: {
    //     key: "grades",
    //     content: "Grades",
    //     icon: "newspaper",
    //   },
    //   render: () => <Tab.Pane></Tab.Pane>,
    // },
    {
      menuItem: {
        key: "user",
        content: "Students",
        icon: "users",
      },
      render: () => (
        <Tab.Pane>
          <CourseUsersList />
        </Tab.Pane>
      ),
    },
  ];
  const CourseManagementDashboardTabs = () => (
    <Tab menu={{ pointing: true, secondary: true }} panes={panes} />
  );

  return (
    <Segment>
      <CourseManagementDashboardTabs />
    </Segment>
  );
};

export default observer(CourseManagementDashboard);
