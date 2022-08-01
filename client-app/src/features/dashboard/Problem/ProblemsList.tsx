import { observer } from "mobx-react-lite";
import React, { useContext, useEffect, useState } from "react";
import { Pagination, Table } from "semantic-ui-react";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import { IProblemSummary } from "../../../app/models/problemSummary";
import { RootStoreContext } from "../../../app/stores/rootStore";
import ProblemListItem from "./ProblemListItem";

const ProblemList: React.FC = () => {
  const pageSize = 11;
  const rootStore = useContext(RootStoreContext);
  const {
    problemSummariesById,
    loadProblems,
    loadingInitial,
    deleteProblem,
    submitting,
    target,
  } = rootStore.problemStore;
  const { hasRole } = rootStore.userStore;
  const [activePage, setActivePage] = useState(1);
  useEffect(() => {
    loadProblems();
  }, [loadProblems]);

  if (loadingInitial) {
    return <LoadingComponent />;
  }

  const handlePageChange = (event: any, data: any) => {
    setActivePage(data.activePage);
  };

  return (
    <Table celled selectable>
      <Table.Header>
        <Table.Row>
          <Table.HeaderCell
            colSpan="4"
            textAlign="center"
            style={{ fontSize: "20px" }}
          >
            Problems List
          </Table.HeaderCell>
        </Table.Row>
        <Table.Row>
          <Table.HeaderCell>ID</Table.HeaderCell>
          <Table.HeaderCell>Name</Table.HeaderCell>
          <Table.HeaderCell>Difficulty</Table.HeaderCell>
          <Table.HeaderCell>Tags</Table.HeaderCell>
          <Table.HeaderCell></Table.HeaderCell>
        </Table.Row>
      </Table.Header>
      <Table.Body>
        {problemSummariesById
          .slice((activePage - 1) * pageSize, activePage * pageSize)
          .map((summary: IProblemSummary) => (
            <ProblemListItem
              key={summary.Id}
              summary={summary}
              submitting={submitting}
              target={target}
              deleteProblem={deleteProblem}
              hasRole={hasRole}
            />
          ))}
      </Table.Body>
      <Table.Footer>
        <Table.Row>
          <Table.HeaderCell colSpan="4">
            <Pagination
              inverted
              floated="right"
              onPageChange={handlePageChange}
              boundaryRange={0}
              ellipsisItem={null}
              firstItem={null}
              lastItem={null}
              siblingRange={1}
              totalPages={Math.max(
                1,
                Math.ceil(problemSummariesById.length / pageSize)
              )}
              activePage={activePage}
            />
          </Table.HeaderCell>
        </Table.Row>
      </Table.Footer>
    </Table>
  );
};

export default observer(ProblemList);
