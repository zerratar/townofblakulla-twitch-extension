import * as React from "react";
import BlakullaService from "../blakulla-service";

export interface GameMenuProps {
    service: BlakullaService;
    lynched: boolean;
    mafia: boolean;
    abilityArgs: string[];
}

export interface GameMenuState {
    lastWill: string;
    deathNote: string;
    lastWillVisible: boolean;
    deathNoteVisible: boolean;    
    abilityUseVisible: boolean;
}

export default class GameMenu extends React.Component<GameMenuProps, GameMenuState> {
    private readonly service: BlakullaService;

    constructor(props:GameMenuProps) {
        super(props);

        this.state = { lastWill: "", deathNote: "", lastWillVisible: false, deathNoteVisible: false, abilityUseVisible: false };

        this.service = props.service;
        this.onLastWillChanged = this.onLastWillChanged.bind(this);
        this.onDeathNoteChanged = this.onDeathNoteChanged.bind(this);
        this.toggleLastWill = this.toggleLastWill.bind(this);
        this.toggleDeathNote = this.toggleDeathNote.bind(this);
        this.toggleAbilityUse = this.toggleAbilityUse.bind(this);
        this.updateLastWillAsync = this.updateLastWillAsync.bind(this);
        this.updateDeathNoteAsync = this.updateDeathNoteAsync.bind(this);
    }

    public render() {

        const inputPanel = 
        this.state.abilityUseVisible
            ? this.renderAbilityUse()
            : this.state.lastWillVisible 
                ? this.renderLastWill()
                : this.state.deathNoteVisible && this.props.mafia
                    ? this.renderDeathNote()
                    : null;


        const deathNoteButton = this.props.mafia
            ? (<button onClick={this.toggleDeathNote}>DESU NOTO?</button>)
            : null;
            
        const abilityButton = this.props.abilityArgs && this.props.abilityArgs.length > 0
            ? (<button onClick={this.toggleAbilityUse}>Ability</button>)
            : null;

        return (
            <div className="menu">
                <div className="menu-items">
                    <button onClick={this.toggleLastWill}>LAST WILLY?</button>
                    {deathNoteButton}
                    {abilityButton}
                </div>
                {inputPanel}
            </div>
        );
    }

    private renderDeathNote() {
        return (
            <div className="input-panel">
                <textarea onChange={this.onDeathNoteChanged} value={this.state.deathNote}></textarea>
                <button onClick={this.updateDeathNoteAsync}>Save</button>
            </div>            
        );
    }

    private renderAbilityUse() {
        return (
            <div className="input-panel">
                {this.props.abilityArgs.map((x,i) => {
                    return (<button key={`ability-${i}`} onClick={this.useAbility} data-value={x}>{x}</button>)
                })}
            </div>
        );
    }
    
    private renderLastWill() {
        return (
            <div className="input-panel">
                <textarea onChange={this.onLastWillChanged} value={this.state.lastWill}></textarea>
                <button onClick={this.updateLastWillAsync}>Save</button>
            </div>            
        );
    }
    
    private toggleAbilityUse() {
        this.setState({abilityUseVisible: !this.state.abilityUseVisible, lastWillVisible: false, deathNoteVisible: false});
    }

    private toggleDeathNote() {
        this.setState({deathNoteVisible: !this.state.deathNoteVisible, lastWillVisible: false});
    }

    private toggleLastWill() {
        this.setState({lastWillVisible: !this.state.lastWillVisible, deathNoteVisible: false});
    }

    private onLastWillChanged(event: React.ChangeEvent<HTMLTextAreaElement>): void {
        const elm = event.currentTarget as HTMLTextAreaElement;        
        this.setState({lastWill:elm.value});
    }

    private onDeathNoteChanged(event: React.ChangeEvent<HTMLTextAreaElement>): void {
        const elm = event.currentTarget as HTMLTextAreaElement;        
        this.setState({deathNote:elm.value});
    }
    
    private updateLastWillAsync() {
        if (!this.state.lastWill) return null;
        try {
            return this.service.updateLastWillAsync(this.state.lastWill);
        } finally {
            this.toggleLastWill();
        }
    }

    private updateDeathNoteAsync() {
        if (!this.state.deathNote) return null;
        try {
            return this.service.updateDeathNoteAsync(this.state.deathNote);
        } finally {
            this.toggleDeathNote();
        }
    }

    private useAbility(event: React.MouseEvent<HTMLButtonElement, MouseEvent>): void {
        var ability = event.currentTarget.dataset.value;
        console.log("use ability: " + ability);
    }
}