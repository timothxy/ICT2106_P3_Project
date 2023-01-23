
import React from "react"
import { json } from "react-router-dom"
import { Loading } from "../../Components/appCommon"
import { StdButton } from "../../Components/common"
import DatapageLayout from "../PageLayout"

export default class Permissions extends React.Component {
    state={
        content:null,
        headers:[],
        loading:true,
        settings: {},
        error: "",
    }

    settings ={
        title:"Permissions",
        primaryColor: "#a6192e",
        accentColor: "#94795d",
        textColor: "#ffffff",
        textColorInvert: "#606060",
        api: "/api/Permissions/",
    }

    async componentDidMount(){
        await this.getPermissions().then((permissions)=>{
            console.log(permissions);
            this.setState({
                permissions:permissions.data.Permission,
            });
        })
        await this.getContent().then((content)=>{
            console.log(content);
            this.setState({
                content:content,
            });
        })

        await this.getSettings().then((settings)=>{
            console.log(settings);
            this.setState({
                settings:settings,
            });
        })

        this.setState({
            loading:false,
        })
    }

    getSettings = async () => {
        // fetches http://...:5001/api/User/Settings
        return fetch(this.settings.api + "Settings" , {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
            },
        }).then(res => {
            console.log(res);
            return res.json();
        })
    }

    getContent = async () =>{
        return fetch( this.settings.api + "All" , {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
            },
        }).then(res => {
            console.log(res);
            //Res = {success: true, message: "Success", data: Array(3)}
            return res.json();
        });
    }
    getPermissions = async () =>{
        return fetch( "/api/Permissions/GetPermissions/" + this.props.user.data.Role , {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
            },
        }).then(res => {
            console.log(res);
            //Res = {success: true, message: "Success", data: Array(3)}
            return res.json();
        });
    }

    update = async (data) =>{
        console.log(data);
        return fetch(this.settings.api + "UpdateAndFetch/" + data.Id , {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(data)
        }).then(async res => {
            return res.json();
        });
    }

    handleUpdate = async (data) =>{
        return await this.update(data).then((content)=>{
            if(content.success){
                this.setState({
                    error:"",
                })
                return true;
            }else{
                this.setState({
                    error:content.message,
                })
                return false;
            }
        })
    }

    requestRefresh = async () =>{
        this.setState({
            loading:true,
        })
        await this.getContent().then((content)=>{
            console.log(content);
            this.setState({
                content:content,
                loading:false,
            });
        })
    }

    render(){
        if(this.state.loading){
            return <Loading></Loading>
        }else{
            
        return(
            <DatapageLayout 
                settings={this.settings}
                fieldSettings={this.state.settings.data.FieldSettings} 
                headers={this.state.settings.data.ColumnSettings} 
                data={this.state.content.data}
                updateHandle = {this.handleUpdate}
                requestRefresh = {this.requestRefresh}
                error={this.state.error}
                permissions={this.props.permissions}
                >
                {this.state.content.data.map((item, index) => {
                    return (
                        <div className="staff-extended">
                            <PermissionsMap handleUpdate={this.handleUpdate} item={item}></PermissionsMap>
                        </div>
                    )
                })}
            </DatapageLayout>
            )
        }
    }
}

export class PermissionsMap extends React.Component {
    state= {
        item: this.props.item,
        loading:true,
        message:"",
    }
    componentDidMount(){
        this.setState({
            permission: JSON.parse(this.props.item.Permission),
            loading:false
        })
    }
    onChange = (index,key)=>{
        let permission = this.state.permission;
        permission[index][key] = !permission[index][key]; 
        this.setState({
            permission:permission
        })
    }

    handleUpdate = async () =>{
        var data = this.state.item;
        data.Permission = JSON.stringify(this.state.permission);
        this.setState({
            loading:true
        })
        await this.props.handleUpdate(data).then((res)=>{
            if(res){
                this.setState({
                    loading:false,
                    success:true,
                    message : "Changes saved!"
                })
            }else{
                this.setState({
                    loading:false,
                    success:false,
                    message : "Error saving permissions!"
                })
            }
        })
    }
    render(){
        return(
            this.state.loading ? 
            <Loading></Loading>
            :
            
            <div className="container-fluid g-0">
                {this.state.message != "" ?
                <div 
                    className={"alert " + (this.state.success ? "alert-success" : "alert-danger")} 
                    onClick={()=>{this.setState({message:""})}}
                >{this.state.message}</div>
                :
                <span></span>
                }
                <div className="row">
                    {this.state.permission.map((item, moduleIndex) => {
                            return <div className="py-4 col-md-6 col-12 permission-module-container d-flex flex-column">
                                <div className="permission-module">
                                    {item.Module}
                                </div>
                                <div className="row">
                                {Object.keys(item).slice(1).map((key, index) => {
                                    return(
                                        <div className="d-flex col-3 flex-column permission-toggle-group">
                                            <div className="permission-toggle-label">
                                                {key}
                                            </div>
                                            <div>
                                                <input className="form-check-input permission-toggle" type="checkbox" checked={item[key]} onChange={()=>this.onChange(moduleIndex,key)}></input>
                                            </div>
                                        </div>
                                    )
                                        
                                })}
                                </div>
                            </div>
                    })}
                </div>
                <StdButton onClick={this.handleUpdate}>
                    Save Changes
                </StdButton>
            </div>
        )
    }
}