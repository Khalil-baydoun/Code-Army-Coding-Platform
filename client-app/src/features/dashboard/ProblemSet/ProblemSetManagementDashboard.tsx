import { observer } from "mobx-react-lite";
import React, { useContext, useEffect } from "react";
import { RouteComponentProps } from "react-router";
import { Segment, Tab } from "semantic-ui-react";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import { RootStoreContext } from "../../../app/stores/rootStore";
import EditProblemSetForm from "../../forms/EditProblemSetForm";
import ProblemSetStatistics from "./ProblemSetStatistics";
import UserProblemSetStatistics from "./UserProblemSetStatistics";

interface DetailParams {
  courseId: string;
  problemSetId: string;
}

const ProblemSetManagementDashboard: React.FC<
  RouteComponentProps<DetailParams>
> = ({ match }) => {
  const rootStore = useContext(RootStoreContext);
  const {
    loadProblemSet,
    course,
    getProblemSetStatistics,
    getCourseGroups,
    loadingInitial,
    getProblemSetStatisticsOfGroup,
    cleanUpProblemSetStatisticsRegistry,
  } = rootStore.courseProblemSetStore;

  useEffect(() => {
    loadProblemSet(match.params.courseId, match.params.problemSetId).then(() =>
      getCourseGroups(match.params.courseId).then((cg) => {
        //console.log(cg);
        cg?.forEach((x) => {
          //console.log(x.Id);
          getProblemSetStatisticsOfGroup(match.params.problemSetId, x.Id);
        });
      })
    );
    getProblemSetStatistics(match.params.problemSetId);
  }, [
    match.params.courseId,
    match.params.problemSetId,
    loadProblemSet,
    getProblemSetStatistics,
  ]);

  if (loadingInitial) return <LoadingComponent />;

  if (!course) {
    return <h2>course not found!</h2>;
  }
  const panes = [
    {
      menuItem: {
        key: "edit",
        content: "ProblemSet Edit",
        icon: "edit",
      },
      render: () => (
        <Tab.Pane>
          <EditProblemSetForm />
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
          <ProblemSetStatistics />
          <UserProblemSetStatistics />
        </Tab.Pane>
      ),
    },
  ];
  const ProblemSetManagementDashboardTabs = () => (
    <Tab menu={{ pointing: true, secondary: true }} panes={panes} />
  );

  return (
    <Segment>
      <ProblemSetManagementDashboardTabs />
    </Segment>
  );
};

export default observer(ProblemSetManagementDashboard);
