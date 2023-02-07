import React from "react"
import { Loading } from "../../Components/appCommon";


export default class VolunteerHome extends React.Component{
    state={
        loading:true,
    }

    componentDidMount = async () =>{
        await this.getVolunteerWork().then((data)=>{
            console.log(data);
        })
        this.setState({
            loading:false,
        })
    }

    getVolunteerWork = async () => {
        var loggedInVol = this.props.user.data;
        console.log(loggedInVol.UserId);
        return fetch("/api/VolunteerWork/GetByVolunteerId/" + loggedInVol.UserId ,{
            method: "GET",
            headers:{
                "Content-Type": "application/json",
            },
        }).then(response => {
            return response.json();
        });
    }


    render(){
        return(
            this.state.loading?
            <Loading></Loading>
            :
            <div>Volunteer Home</div>
        )
    }
}