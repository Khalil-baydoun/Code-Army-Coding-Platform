import React from "react";
import { Table } from "semantic-ui-react";

const VerdictsGuide = () => {
  return (
    <div>
      <Table celled definition>
        <Table.Header>
          <Table.Row>
            <Table.HeaderCell
              colSpan="3"
              textAlign="center"
              style={{ fontSize: "20px" }}
            >
              Verdicts Guide
            </Table.HeaderCell>
          </Table.Row>
          <Table.Row>
            <Table.HeaderCell width={2} />
            <Table.HeaderCell>Symbol</Table.HeaderCell>
            <Table.HeaderCell>Description</Table.HeaderCell>
          </Table.Row>
        </Table.Header>

        <Table.Body>
          <Table.Row>
            <Table.Cell>In Queue</Table.Cell>
            <Table.Cell>IQ</Table.Cell>
            <Table.Cell>
              Your submission is added to the judging queue. It will be judged
              as soon as possible.
            </Table.Cell>
          </Table.Row>
          <Table.Row>
            <Table.Cell>Accepted</Table.Cell>
            <Table.Cell>AC</Table.Cell>
            <Table.Cell>
              Your program is correct! It produced the right answer in
              reasoneable time and within the limit memory usage.
              Congratulations!
            </Table.Cell>
          </Table.Row>
          <Table.Row>
            <Table.Cell>Wrong Answer</Table.Cell>
            <Table.Cell>WA</Table.Cell>
            <Table.Cell>
              Correct solution not reached for the inputs. You can see the test
              that failed inside your submission report , but if the test is
              quite large, that will not be much useful. If you truly think your
              code is correct, you can contact your instructor to inform him/her
              about this.
            </Table.Cell>
          </Table.Row>
          <Table.Row>
            <Table.Cell>Runtime Error</Table.Cell>
            <Table.Cell>RE</Table.Cell>
            <Table.Cell>
              Your program failed during the execution, the reason can be one of
              the following: segmentation fault, floating point exception,
              division by zero, modulo by zero, integer overflow, failed
              assertion, index out of bounds, infinite recursion, derefrencing
              null pointers, etc...
            </Table.Cell>
          </Table.Row>
          <Table.Row>
            <Table.Cell>Time Limit Exceeded</Table.Cell>
            <Table.Cell>TLE</Table.Cell>
            <Table.Cell>
              Your program tried to run for more time than the allowed time
              limit of the problem
            </Table.Cell>
          </Table.Row>
          <Table.Row>
            <Table.Cell>Memory Limit Exceeded</Table.Cell>
            <Table.Cell>MLE</Table.Cell>
            <Table.Cell>
              Your program tried to take more memory than the memory limit of
              the problem
            </Table.Cell>
          </Table.Row>
        </Table.Body>
      </Table>
    </div>
  );
};

export default VerdictsGuide;
