// import React, { Fragment, useContext } from "react";
// import { Link } from "react-router-dom";
// import { Button, Container, Header, Segment } from "semantic-ui-react";
// import { RootStoreContext } from "../../app/stores/rootStore";
// import LoginForm from "../forms/LoginForm";
// import SignUpForm from "../forms/SignUpForm";

// const HomePage = () => {
//   const rootStore = useContext(RootStoreContext);
//   const { user, isLoggedIn } = rootStore.userStore;
//   const { openModal } = rootStore.modalStore;
//   return (
//     <Segment inverted textAlign="center" vertical className="masthead">
//       <link
//         href="https://fonts.googleapis.com/css2?family=Lato:ital,wght@0,100;0,300;0,400;1,300&display=swap"
//         rel="stylesheet"
//         type="text/css"
//       />
//       <img
//         src="/./assets/images/AUBlogo.png"
//         alt="AUB logo"
//         width="170"
//         style={{ position: "absolute", top: "8px", left: "16px" }}
//       ></img>
//       <Container>
//         <Header as="h1" className="Title">
//           <span>AUB Coding Platform</span>
//         </Header>
//         <Header
//           as="h2"
//           inverted
//           content="Start Coding Now!"
//           style={{ marginBottom: "25px", fontWeight: 370, fontSize: "210%" }}
//         />
//         {isLoggedIn && user ? (
//           <Fragment>
//             <Header
//               as="h2"
//               inverted
//               content={`Welcome back ${user.FirstName}`}
//             />
//             <Button as={Link} to="/course" size="huge" inverted>
//               Go to Courses!
//             </Button>
//           </Fragment>
//         ) : (
//           <Fragment>
//             <Button
//               size="huge"
//               inverted
//               onClick={() => openModal(<LoginForm />)}
//               style={{ marginRight: "30px", fontWeight: 400 }}
//             >
//               Login
//             </Button>
//             <Button
//               size="huge"
//               onClick={() => openModal(<SignUpForm />)}
//               style={{ fontWeight: 400 }}
//             >
//               Sign Up
//             </Button>
//           </Fragment>
//         )}
//       </Container>
//     </Segment>
//   );
// };
import React from "react";
// reactstrap components
import { Button, Col, Container, Row } from "reactstrap";
import DefaultFooter from "./components/Footers/DefaultFooter";
import LandingPageHeader from "./components/Headers/LandingPageHeader";
// core components
import ExamplesNavbar from "./components/Navbars/ExamplesNavbar";

// import "./assets/css/bootstrap.min.css";
// import "./assets/scss/now-ui-kit.scss?v=1.4.0";
// import "./assets/demo/demo.css?v=1.4.0";
// import "./assets/demo/nucleo-icons-page-styles.css?v=1.4.0";

function HomePage() {
  const [firstFocus, setFirstFocus] = React.useState(false);
  const [lastFocus, setLastFocus] = React.useState(false);
  React.useEffect(() => {
    // document.body.classList.add("landing-page");
    // document.body.classList.add("sidebar-collapse");
    // document.documentElement.classList.remove("nav-open");
    window.scrollTo(0, 0);
    document.body.scrollTop = 0;
    // return function cleanup() {
    //   document.body.classList.remove("landing-page");
    //   document.body.classList.remove("sidebar-collapse");
    // };
  }, []);
  require("./assets/css/bootstrap.min.css");
  require("./assets/scss/now-ui-kit.scss?v=1.4.0");
  require("./assets/demo/demo.css?v=1.4.0");
  require("./assets/demo/nucleo-icons-page-styles.css?v=1.4.0");
  return (
    <>
      {/* {require("./assets/css/bootstrap.min.css")}
      {require("./assets/scss/now-ui-kit.scss?v=1.4.0")}
      {require("./assets/demo/demo.css?v=1.4.0")}
      {require("./assets/demo/nucleo-icons-page-styles.css?v=1.4.0")} */}

      <ExamplesNavbar />
      <div className="wrapper">
        <LandingPageHeader />
        <div className="section section-about-us">
          <Container>
            <Row>
              <Col className="ml-auto mr-auto text-center" md="8">
                <h2 className="title">Who we are?</h2>
                <h5 className="description">
                  We are a team of CCE students at AUB, who decided to develop
                  this Website for our Final Year Project. Advised by Dr. Louay
                  Bazzi we developed an online judge that allows students to
                  submit their labs and practice coding and get feedback in
                  seconds.
                </h5>
              </Col>
            </Row>
            <div className="separator separator-primary"></div>
            <div className="section-story-overview">
              <Row>
                <Col md="6">
                  <div
                    className="image-container image-left"
                    style={{
                      backgroundImage:
                        "url(" + require("./assets/img/code-gif-2.gif") + ")",
                    }}
                  >
                    <p className="blockquote blockquote-info">
                      "Using React, Typescript, Bootstrap, SASS, MobX for the
                      frontend, as well as .NET Core with Entity Framework for
                      the backend and database. We also used SQLite for
                      development, and Azure SQL storage for deployment and
                      production. We have learned a lot!" <br></br>
                      <br></br>
                    </p>
                  </div>
                  <div
                    className="image-container"
                    style={{
                      backgroundImage:
                        "url(" + require("./assets/img/teaching.jpg") + ")",
                    }}
                  ></div>
                </Col>
                <Col md="5">
                  <div
                    className="image-container image-right"
                    style={{
                      backgroundImage:
                        "url(" + require("./assets/img/programming.jpg") + ")",
                    }}
                  ></div>
                  <h3>
                    So what does this new website add to the experience of AUB
                    students
                  </h3>
                  <p>
                    In introductory programming courses, correcting programming
                    codes takes a significant amount of time by the lab
                    instructors, which eventually results in a delayed feedback
                    given back to the students. In addition to that, the
                    feedback is often just a grade or a one sentence comment
                    which is not enough.
                  </p>
                  <p>
                    And because of that, the coding learning experience becomes
                    longer and harder on the students, which forces them to get
                    the support of the internet a lot of times to fix the issues
                    they face while coding, or just submit a wrong lab
                    submission.
                  </p>
                  <p>
                    We experienced that, and now we are graduating we know how
                    important it is to learn to write and debug code from
                    computer engineering perspective, and how the industry is
                    heavily dependent on coding. Hence through our project we
                    aimed to solve this problem by providing real time feedback
                    to students and guiding them to figure out what is wrong
                    with their code, which will ease up their learning process
                    and save their time.
                  </p>
                </Col>
              </Row>
            </div>
          </Container>
        </div>
        <div className="section section-team text-center">
          <Container>
            <h2 className="title">Here is our team</h2>
            <div className="team">
              <Row>
                <Col md="3">
                  <div className="team-player">
                    <img
                      alt="..."
                      className="rounded-circle img-fluid img-raised"
                      src={require("./assets/img/khalil.jpg")}
                    ></img>
                    <h4 className="title">Khalil Baydoun</h4>
                    <p className="category text-primary">
                      Full Stack Developer & UI Designer
                    </p>
                    <p className="description"></p>
                    <Button
                      className="btn-icon btn-round"
                      color="primary"
                      href="#pablo"
                      onClick={(e) => e.preventDefault()}
                    >
                      <i className="fab fa-twitter"></i>
                    </Button>
                    <Button
                      className="btn-icon btn-round"
                      color="secondary"
                      href="#pablo"
                      onClick={(e) => e.preventDefault()}
                    >
                      <i className="fab fa-instagram"></i>
                    </Button>
                    <Button
                      className="btn-icon btn-round"
                      color="tertiary"
                      href="#pablo"
                      onClick={(e) => e.preventDefault()}
                    >
                      <i className="fab fa-facebook-square"></i>
                    </Button>
                  </div>
                </Col>
                <Col md="3">
                  <div className="team-player">
                    <img
                      alt="..."
                      className="rounded-circle img-fluid img-raised"
                      src={require("./assets/img/abed.jpeg")}
                    ></img>
                    <h4 className="title">Abed Al Mottaleb Al Amin</h4>
                    <p className="category text-primary">
                      Backend Developer - Asp.net Core
                    </p>
                    <p className="description"></p>
                    <Button
                      className="btn-icon btn-round"
                      color="primary"
                      href="#pablo"
                      onClick={(e) => e.preventDefault()}
                    >
                      <i className="fab fa-twitter"></i>
                    </Button>
                    <Button
                      className="btn-icon btn-round"
                      color="secondary"
                      href="#pablo"
                      onClick={(e) => e.preventDefault()}
                    >
                      <i className="fab fa-linkedin"></i>
                    </Button>
                    <Button
                      className="btn-icon btn-round"
                      color="tertiary"
                      href="#pablo"
                      onClick={(e) => e.preventDefault()}
                    >
                      <i className="fab fa-facebook-square"></i>
                    </Button>
                  </div>
                </Col>
                <Col md="3">
                  <div className="team-player">
                    <img
                      alt="..."
                      className="rounded-circle img-fluid img-raised"
                      src={require("./assets/img/hussein.jpeg")}
                    ></img>
                    <h4 className="title">Hussein Mokdad</h4>
                    <p className="category text-primary">
                      Frontend Developer & UI Designer
                    </p>
                    <p className="description"></p>
                    <Button
                      className="btn-icon btn-round"
                      color="primary"
                      href="#pablo"
                      onClick={(e) => e.preventDefault()}
                    >
                      <i className="fab fa-google-plus"></i>
                    </Button>
                    <Button
                      className="btn-icon btn-round"
                      color="secondary"
                      href="#pablo"
                      onClick={(e) => e.preventDefault()}
                    >
                      <i className="fab fa-youtube"></i>
                    </Button>
                    <Button
                      className="btn-icon btn-round"
                      color="tertiary"
                      href="#pablo"
                      onClick={(e) => e.preventDefault()}
                    >
                      <i className="fab fa-twitter"></i>
                    </Button>
                  </div>
                </Col>
                <Col md="3">
                  <div className="team-player">
                    <img
                      alt="..."
                      className="rounded-circle img-fluid img-raised"
                      src={require("./assets/img/hassan.jpeg")}
                    ></img>
                    <h4 className="title">Hasan Abbass</h4>
                    <p className="category text-primary">
                      Backend Developer - Database Management
                    </p>
                    <p className="description"></p>
                    <Button
                      className="btn-icon btn-round"
                      color="primary"
                      href="#pablo"
                      onClick={(e) => e.preventDefault()}
                    >
                      <i className="fab fa-google-plus"></i>
                    </Button>
                    <Button
                      className="btn-icon btn-round"
                      color="secondary"
                      href="#pablo"
                      onClick={(e) => e.preventDefault()}
                    >
                      <i className="fab fa-youtube"></i>
                    </Button>
                    <Button
                      className="btn-icon btn-round"
                      color="tertiary"
                      href="#pablo"
                      onClick={(e) => e.preventDefault()}
                    >
                      <i className="fab fa-twitter"></i>
                    </Button>
                  </div>
                </Col>
              </Row>
            </div>
          </Container>
        </div>
        {/* <div className="section section-contact-us text-center">
          <Container>
            <h2 className="title">You have an idea?</h2>
            <p className="description">
              Your feedback is very important to us.
            </p>
            <Row>
              <Col className="text-center ml-auto mr-auto" lg="6" md="8">
                <InputGroup
                  className={
                    "input-lg" + (firstFocus ? " input-group-focus" : "")
                  }
                >
                  <InputGroupAddon addonType="prepend">
                    <InputGroupText>
                      <i className="now-ui-icons users_circle-08"></i>
                    </InputGroupText>
                  </InputGroupAddon>
                  <Input
                    placeholder="First Name..."
                    type="text"
                    onFocus={() => setFirstFocus(true)}
                    onBlur={() => setFirstFocus(false)}
                  ></Input>
                </InputGroup>
                <InputGroup
                  className={
                    "input-lg" + (lastFocus ? " input-group-focus" : "")
                  }
                >
                  <InputGroupAddon addonType="prepend">
                    <InputGroupText>
                      <i className="now-ui-icons ui-1_email-85"></i>
                    </InputGroupText>
                  </InputGroupAddon>
                  <Input
                    placeholder="Email..."
                    type="text"
                    onFocus={() => setLastFocus(true)}
                    onBlur={() => setLastFocus(false)}
                  ></Input>
                </InputGroup>
                <div className="textarea-container">
                  <Input
                    cols="80"
                    name="name"
                    placeholder="Type a message..."
                    rows="4"
                    type="textarea"
                  ></Input>
                </div>
                <div className="send-button">
                  <Button
                    block
                    className="btn-round"
                    color="secondary"
                    href="#pablo"
                    onClick={(e) => e.preventDefault()}
                    size="lg"
                  >
                    Send Message
                  </Button>
                </div>
              </Col>
            </Row>
          </Container>
        </div> */}
        <DefaultFooter />
      </div>
    </>
  );
}

export default HomePage;
