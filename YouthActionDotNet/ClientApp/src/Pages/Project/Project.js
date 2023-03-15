import React from "react";
import { useState, useEffect, useRef } from "react";
import { Loading } from "../../Components/appCommon";
import DatapageLayout from "../PageLayoutEmpty";
import Table from "react-bootstrap/Table";
import { useParams } from "react-router-dom";
import { BrowserRouter as Router, Link, Switch, Route } from "react-router-dom";
import { ToastContainer, toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { useNavigate } from "react-router-dom";

export default class Project extends React.Component {
  state = {
    content: null,
    headers: [],
    loading: true,
    settings: {},
    error: "",
  };

  settings = {
    title: "Project",
    primaryColor: "#a6192e",
    accentColor: "#94795d",
    textColor: "#ffffff",
    textColorInvert: "#606060",
    api: "/api/Project/",
  };
  has = {
    Create: true,
    Generate: false,
    Delete: false,
  };

  async componentDidMount() {
    await this.getContent().then((content) => {
      console.log(content);
      this.setState({
        content: content,
      });
    });

    await this.getSettings().then((settings) => {
      console.log(settings);
      this.setState({
        settings: settings,
      });
    });

    this.setState({
      loading: false,
    });
  }

  getSettings = async () => {
    // fetches http://...:5001/api/User/Settings
    return fetch(this.settings.api + "Settings", {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    }).then((res) => {
      console.log(res);
      return res.json();
    });
  };

  getContent = async () => {
    return fetch(this.settings.api + "All", {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    }).then((res) => {
      console.log(res);
      //Res = {success: true, message: "Success", data: Array(3)}
      return res.json();
    });
  };

  update = async (data) => {
    console.log(data);
    return fetch(`${this.settings.api}UpdateAndFetch/${data.ProjectId}`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    }).then(async (res) => {
      return res.json();
    });
  };
  create = async (data) => {
    return fetch("https://localhost:5001/api/Project/Create", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    }).then(function (res) {
      return res.json();
    });
  };
  delete = async (data) => {
    await fetch(`https://localhost:5001/api/Project/Delete/${data}`, {
      method: "DELETE",
      headers: { "Content-Type": "application/json" },
    }).then(async (res) => {
      return res.json();
    });
  };

  handleUpdate = async (data) => {
    await this.update(data).then((content) => {
      if (content.success) {
        this.setState({
          error: "",
        });
        return true;
      } else {
        this.setState({
          error: content.message,
        });
        return false;
      }
    });
  };

  handleDelete = async (data) => {
    await this.delete(data).then((content) => {
      if (content.success) {
        this.setState({
          error: "",
        });
        return true;
      } else {
        this.setState({
          error: content.message,
        });
        return false;
      }
    });
  };

  requestRefresh = async () => {
    this.setState({
      loading: true,
    });
    await this.getContent().then((content) => {
      console.log(content);
      this.setState({
        content: content,
        loading: false,
      });
    });
  };

  requestError = async (error) => {
    this.setState({
      error: error,
    });
  };

  render() {
    if (this.state.loading) {
      return <Loading></Loading>;
    } else {
      return (
        <DatapageLayout
          settings={this.settings}
          fieldSettings={this.state.settings.data.FieldSettings}
          headers={this.state.settings.data.ColumnSettings}
          data={this.state.content.data}
          updateHandle={this.handleUpdate}
          requestRefresh={this.requestRefresh}
          error={this.state.error}
          permissions={this.props.permissions}
          requestError={this.requestError}
          has={this.has}
        >
          <ProjectTable
            data={this.state.content.data}
            delete={this.delete}
            create={this.create}
            requestRefresh={this.requestRefresh}
          />
        </DatapageLayout>
      );
    }
  }
}

const ProjectTable = (props) => {
  const data = props.data;
  const deleteFn = props.delete;
  const createFn = props.create;
  let navigate = useNavigate();
  const routeChange = (id) => {
    let path = `/Project/Edit/${id}`;
    navigate(path);
  };
  const routeReturn = () => {
    let path = `/Project`;
    navigate(path);
  };

  const countRef = useRef();
  const [projects, setProjects] = useState([]);
  const [projRef, setProjRef] = useState();
  const [undo, setUndo] = useState(false);
  const [sorting, setSorting] = useState({
    field: "ProjectName",
    ascending: true,
  });
  const params = useParams();
  console.log(params);
  if (params.id) {
    console.log(params.id);
  } else {
    console.log("no params");
  }
  const options = {
    onClose: () => {
      console.log(countRef);
      if (countRef.current == null) {
        deleteFn(params.id);
      }
    },
    autoClose: 2000,
    hideProgressBar: true,
    className: "black-background",
    position: toast.POSITION.BOTTOM_CENTER,
  };

  //When delete is clicked, all details of the project will be copied and stored in temporary variable.
  useEffect(() => {
    // setProjects(data);
    const projectsCopy = [...data];
    const projectsFiltered = projectsCopy.filter((project) => {
      return project.ProjectId != params.id;
    });

    console.log(projectsFiltered);
    // Apply sorting
    let sortedProjects = [];
    if (undo == true) {
      sortedProjects = projectsCopy
        .sort((a, b) => {
          console.log(a[sorting.field]);
          console.log(b[sorting.field]);
          if (typeof b[sorting.field] == "string") {
            if (b) {
              return a ? b[sorting.field]?.localeCompare(a[sorting.field]) : 1;
            } else if (a) {
              return b ? a[sorting.field]?.localeCompare(b[sorting.field]) : -1;
            }
          } else if (typeof b[sorting.field] == "number") {
            if (b) {
              return a ? b[sorting.field] - a[sorting.field] : 1;
            } else if (a) {
              return b ? a[sorting.field] - b[sorting.field] : -1;
            }
          }
        })
        .slice();
    } else {
      setProjRef(
        projectsCopy.filter((project) => {
          return project.ProjectId == params.id;
        })
      );
      sortedProjects = projectsFiltered
        .sort((a, b) => {
          console.log(a[sorting.field]);
          console.log(typeof b[sorting.field]);
          if (typeof b[sorting.field] == "string") {
            if (b) {
              return a ? b[sorting.field]?.localeCompare(a[sorting.field]) : 1;
            } else if (a) {
              return b ? a[sorting.field]?.localeCompare(b[sorting.field]) : -1;
            }
          } else if (typeof b[sorting.field] == "number") {
            if (b) {
              return a ? b[sorting.field] - a[sorting.field] : 1;
            } else if (a) {
              return b ? a[sorting.field] - b[sorting.field] : -1;
            }
          }
        })
        .slice();
    }
    // Replace currentprojects with sorted currentprojects
    setProjects(
      // Decide either currentprojects sorted by ascending or descending order
      sorting.ascending ? sortedProjects : sortedProjects.reverse()
    );
  }, [data, sorting, undo]);
  useEffect(() => {
    if (data.length > projects.length && projects.length != 0) {
      notify();
      //   console.log(data.length)
      // console.log(projects.length)
    }
  }, [projects]);
  //Toast Message is created to allow users to undo deletion of project
  const notify = () => {
    toast.info(undoToastBtn, options);
  };
  const undoToastBtn = () => (
    <button
      ref={countRef}
      onClick={() => {
        setUndo(true);
        const proj = {
          ProjectName: projRef[0].ProjectName,
          ProjectDescription: projRef[0].ProjectDescription,
          ProjectBudget: projRef[0].ProjectBudget,
          ProjectStartDate: projRef[0].ProjectStartDate,
          ProjectEndDate: projRef[0].ProjectEndDate,
          ProjectStatus: projRef[0].ProjectStatus,
          ProjectCompletionDate: projRef[0].ProjectCompletionDate,
          ProjectType: projRef[0].ProjectType,
          ServiceCenterId: projRef[0].ServiceCenterId,
        };
        console.log(proj);
        handleCreate(proj);
        routeReturn();
      }}
    >
      Undo
    </button>
  );

  const handleCreate = async (proj) => await createFn(proj);

  function applySorting(key, ascending) {
    setSorting({ field: key, ascending: ascending });
  }

  console.log(projects);
  return (
    <>
    <Table striped bordered hover>
      <thead>
        <tr>
          <th>#</th>
          <th onClick={() => applySorting("ProjectName", !sorting.ascending)}>
            Project Name
          </th>
          <th
            colSpan={2}
            onClick={() =>
              applySorting("ProjectDescription", !sorting.ascending)
            }
          >
            Project Description
          </th>
          <th onClick={() => applySorting("ProjectBudget", !sorting.ascending)}>
            Project Budget
          </th>
          <th
            onClick={() => applySorting("ProjectStartDate", !sorting.ascending)}
          >
            Start Date
          </th>
          <th
            onClick={() => applySorting("ProjectEndDate", !sorting.ascending)}
          >
            End Date
          </th>
          <th onClick={() => applySorting("ProjectStatus", !sorting.ascending)}>
            Project Status
          </th>
          <th></th>
        </tr>
      </thead>
      {projects.map((item, key) => {
        return (
          <tbody key={key}>
            <tr>
              <td>{key + 1}</td>
              <td>{item.ProjectName}</td>
              <td colSpan={2}>{item.ProjectDescription}</td>
              <td>{item.ProjectBudget}</td>
              <td>{item.ProjectStartDate}</td>
              <td>{item.ProjectEndDate}</td>
              <td>{item.ProjectStatus}</td>
              <td>
                <button onClick={() => routeChange(item.ProjectId)}>
                  View
                </button>
                {/* <button onClick={() => setUndo(true)}>Undo</button> */}
              </td>
            </tr>
          </tbody>
        );
      })}
     
    </Table>
     <ToastContainer theme="dark" /></>
    
  );
};
