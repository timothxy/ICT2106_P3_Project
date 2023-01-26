import React from 'react';
import { Loading } from '../../Components/appCommon';
import { StdButton } from '../../Components/common';
import { StdInput } from '../../Components/input';

export default class VolunteerRegistration extends React.Component{
    state ={
        loading:true,
        settings: {},        
        excludes:["UserId","ApprovedBy","ApprovalStatus","Role"]
    }
    
    async componentDidMount(){
        await this.getSettings().then((settings)=>{
            this.setState({
                settings:settings.data,
            })
        })
        this.setState({
            loading:false,
        })
    }

    getSettings = async () => {
        
        return await fetch("/api/Volunteer/Settings",{
            method: "GET",
            headers: {
                "Content-Type": "application/json",
            }
        }).then(response => {
            return response.json();
        });
    }



    render(){
        return(
            this.state.loading ? 
            <Loading></Loading>
            :
            <div className='container'>
                {Object.keys(this.state.settings.FieldSettings).map((key,index)=>{
                const field = this.state.settings.FieldSettings[key];
                console.log(field);
                
                return (
                    this.state.excludes.includes(key) ? null :
                    <div className='row'>
                        <div className='col-12'>
                            <StdInput 
                                label = {field.displayLabel}
                                type={field.type}
                                enabled = {true}
                                fieldLabel={field.fieldLabel}
                                onChange = {this.onChange}
                                options={field.options}
                                dateFormat = {field.dateFormat}
                                allowEmpty = {true}
                                toolTip = {field.toolTip}
                                >
                            </StdInput>
                        </div>
                    </div>
                )
                })}
                <StdButton>Submit</StdButton>
            </div>
        )
    }
}

