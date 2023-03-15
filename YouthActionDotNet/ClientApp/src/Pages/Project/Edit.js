import React from "react";
import { useState, useEffect } from "react";
import { Loading } from "../../Components/appCommon";
import DatapageLayout from "../PageLayoutEmpty";
import Table from "react-bootstrap/Table";

export default class Edit extends React.Component {
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

//   delete = async (data) => {
//     await fetch(`https://localhost:5001/api/Project/Delete/${data}`, {
//       method: "DELETE",
//       headers: { "Content-Type": "application/json" },
//     }).then(async (res) => {
//       location.href = `/Project`;
//     });
//   };
  delete = (data) =>{
    window.location.href = `/Project/${data}`;
  }

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
      console.log(this.state.content.data);
      const id = window.location.href.split("/")[5];
      const data = this.state.content.data.filter((item) => {
        return item.ProjectId == id;
      });
      return (
        <DatapageLayout
          settings={this.settings}
          fieldSettings={this.state.settings.data.FieldSettings}
          headers={this.state.settings.data.ColumnSettings}
          data={data}
          updateHandle={this.handleUpdate}
          requestRefresh={this.requestRefresh}
          error={this.state.error}
          permissions={this.props.permissions}
          requestError={this.requestError}
          has={this.has}
        >
          <ProjectTable
            data={data[0]}
            delete={this.delete}
          />
        </DatapageLayout>
      );
    }
  }
}

const ProjectTable = (props) => {
  const data = props.data;
  console.log(data)
  const deleteFn = props.delete;
  return (
    <Table striped bordered hover>
      <thead>
        <tr>
          <th>#</th>
          <th>Project Name</th>
          <th colSpan={2}>Project Description</th>
          <th>Project Budget</th>
          <th>Start Date</th>
          <th>End Date</th>
          <th>Project Status</th>
          <th></th>
        </tr>
      </thead>
      <tbody>
        <tr>
          <td></td>
          <td>{data.ProjectName}</td>
          <td colSpan={2}>{data.ProjectDescription}</td>
          <td>{data.ProjectBudget}</td>
          <td>{data.ProjectStartDate}</td>
          <td>{data.ProjectEndDate}</td>
          <td>{data.ProjectStatus}</td>
          <td>
            <button
              onClick={() => {
                deleteFn(data.ProjectId);
              }}
            >
              Delete
            </button>
          </td>
        </tr>
      </tbody>
    </Table>
  );
};
