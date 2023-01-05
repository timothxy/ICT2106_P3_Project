
import React from "react"
import DatapageLayout from "./PageLayout"

export default class Users extends React.Component {
    state={
        content:null,
        headers:[],
        loading:true,
        settings: {},
        error: "",
    }

    settings ={
        title:"Users",
        primaryColor: "#a6192e",
        accentColor: "#94795d",
        textColor: "#ffffff",
        textColorInvert: "#606060",
        api: "/api/User/",
    }

    async componentDidMount(){
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
            return res.json();
        });
    }

    update = async (data) =>{
        console.log(data);
        return fetch(this.settings.api + data.UserId , {
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
        await this.update(data).then((content)=>{
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
            return <div>Loading</div>
        }else{
            
        return(
            <DatapageLayout 
                settings={this.settings}
                fieldSettings={this.state.settings.FieldSettings} 
                headers={this.state.settings.ColumnSettings} 
                data={this.state.content.data}
                updateHandle = {this.handleUpdate}
                requestRefresh = {this.requestRefresh}
                error={this.state.error}>
            </DatapageLayout>
            
            )
        }
    }
}