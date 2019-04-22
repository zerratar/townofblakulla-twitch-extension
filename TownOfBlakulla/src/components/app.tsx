import * as React from "react";

import GameState, { GameInfo } from "../game-state";
import BlakullaService, { ChatMessage, GameStateType, Subscription } from "../blakulla-service";
import { ToggleOverlayButton } from "./toggle-overlay-button";

import "./app.scss";
import Day from "./phases/day";
import Night from "./phases/night";
import GameMenu from "./game-menu";

export interface AppProps { }

export class App extends React.Component<AppProps, GameState> {
    private readonly service: BlakullaService;
    private chatSubscription: Subscription<ChatMessage> = null;
    stateTimer: number;
    chatTimer: number;
    
    constructor(props: AppProps) {
        super(props);
        this.service = new BlakullaService();
        this.state = new GameState();
        this.leaveGameAsync = this.leaveGameAsync.bind(this);
        this.joinGameAsync = this.joinGameAsync.bind(this);
        this.onNameChanged = this.onNameChanged.bind(this);
        this.onNameKeyDown = this.onNameKeyDown.bind(this);
        this.visibilityChanged = this.visibilityChanged.bind(this);
    }

    // <div className={this.state.theme === "light" ? "App-light" : "App-dark"} >
    //     <p>{JSON.stringify(this.state.test)}</p>
    //     <p>Hello world!</p>
    //     <p>My token is: {this.service.auth.state.token}</p>
    //     <p>My opaque ID is {this.service.auth.getOpaqueId()}.</p>
    //     <div>{this.service.auth.isModerator() ? <p>I am currently a mod, and here's a special mod button <input value='mod button' type='button' /></p> : 'I am currently not a mod.'}</div>
    //     <p>I have {this.service.auth.hasSharedId() ? `shared my ID, and my user_id is ${this.service.auth.getUserId()}` : 'not shared my ID'}.</p>
    // </div>

    render() {

        if (!this.service.isReady || this.state.state == GameStateType.INVALID || this.state.state == GameStateType.NOT_STARTED) {
            return (<div className="App"></div>);
        }

        if (this.state.isVisible) {

            if (this.state.joined) {
                return this.renderGame();
            }

            if (this.state.state == GameStateType.JOINABLE) {
                return this.renderJoinGame();
            }

            if (this.state.state == GameStateType.STARTED) {
                return this.renderStartedGame();
            }

            return this.renderInvalidGameState(this.state.state);
        }

        return (
            <div className="App">
                <ToggleOverlayButton onVisibilityChanged={this.visibilityChanged} />
            </div>);
    }

    renderGame() {
        let leave = <button onClick={this.leaveGameAsync}>Leave</button>;
        let phase = null;
        
        if (this.state.game != null) {
            switch(this.state.game.phase) {
                case "Day":                    
                    phase = <Day 
                        service={this.service} 
                        gameState={this.state} 
                        channel={this.getDayChatChannel()} />;
                    break;
                case "Night":
                    phase = <Night 
                        service={this.service} 
                        gameState={this.state} 
                        channel={this.getNightChatChannel()} />;
                    break;
            }
        }

        if (this.state.waitForLeave) {
            leave = <p></p>;
            phase = <p></p>;
        }      
        
        if (this.state.joined && !this.state.lynched) {

        }
        const isMafia = this.service.isMafia(this.state.role);
        return (<div className="App">
                    <ToggleOverlayButton onVisibilityChanged={this.visibilityChanged} />
                    <GameMenu service={this.service} lynched={this.state.lynched} mafia={isMafia} />
                    {leave}
                    {phase}                
                </div>);
    }

    renderJoinGame() {

        let join = 
            (<div className="join-panel-input">
                <img className="bg-image" src="./images/frame-tiny.png"></img>
                <input placeholder="Enter a name" value={this.state.name} onChange={this.onNameChanged} onKeyDown={this.onNameKeyDown} />
                <button onClick={this.joinGameAsync}>Join</button>
            </div>);

        if (this.state.waitForJoin || this.state.joined) {
            join = <p>Loading</p>;
        }

        return (
            <div className="App">
                <ToggleOverlayButton onVisibilityChanged={this.visibilityChanged} />
                <div className="join-panel">
                    <div className="game-logo">
                        <img src="./images/logo.png"></img>
                    </div>
                    {join}
                </div>
            </div>
        )
    }

    renderStartedGame() {
        return (
            <div className="App">
                <ToggleOverlayButton onVisibilityChanged={this.visibilityChanged} />
                <div>Game has already started.</div>
            </div>
        )
    }


    renderInvalidGameState(state: number) {
        return (
            <div className="App">
                <ToggleOverlayButton onVisibilityChanged={this.visibilityChanged} />
                <div>Invalid game state: {state}</div>
            </div>
        )
    }

    contextUpdate(context: any, delta: any) {
        if (delta.includes('theme')) {
            this.setState(() => {
                return { theme: context.theme }
            })
        }
    }

    visibilityChanged(isVisible: boolean) {
        this.setState(() => {
            return {
                isVisible
            }
        })
    }

    componentDidMount() {
        
        if (this.chatSubscription) this.chatSubscription.unsubscribe();
        this.chatSubscription = this.service.chatMessageEvent.subscribe(x => this.addChatMessage(x));   

        this.service.loadingEvent.subscribe(x => this.forceUpdate());
        this.service.start((auth: any) => {

            this.updateGameState();
     
            // this.service.leaveAsync()
            // this.service.joinAsync()            
            // this.service.voteAsync(player)
            // this.service.voteAsync(guilty|innocent)
            // this.service.updateLastWillAsync(text)
            // this.service.updateDeathNoteAsync(text)
            // this.service.useAbilityAsync(id,arg0,arg1,argN...)
            // this.service.canUseAbilityAsync()
            this.forceUpdate();
        },
            (visibility: boolean) => this.visibilityChanged(visibility),
            (context: any, delta: any) => this.contextUpdate(context, delta));
    }
    
    componentWillUnmount() {
        if (this.stateTimer) window.clearTimeout(this.stateTimer);
        if (this.chatTimer) window.clearTimeout(this.chatTimer);
        this.service.dispose();
    }

    onNameChanged(e: React.ChangeEvent<HTMLInputElement>) {
        const name = e.currentTarget.value || "";
        this.setState(() => {
            return { name };
        });
    }

    onNameKeyDown(e: React.KeyboardEvent<HTMLInputElement>) {
        if (e.keyCode == 13) {
            e.preventDefault();
            this.joinGameAsync();
        }
    }

    async joinGameAsync() {
        if (!this.state.name || this.state.name == null || this.state.name.trim().length == 0) {
            return;
        }        

        this.setState(()=>{ return { waitForJoin: true }; });

        const result = await this.service.joinAsync(this.state.name);

        const joined = !!result.name;
        const name = result.name || "";
        const role = result.role;

        if (joined) {
            this.rescheduleGetChatMessages();            
        }
        
        this.setState(()=>{ return { waitForJoin: false, joined, name, role }; });

        console.log(result);
    }

    async leaveGameAsync() {
        
        this.setState(() => { return { waitForLeave: true }; })

        if (this.chatTimer) window.clearTimeout(this.chatTimer);

        const result = await this.service.leaveAsync(); // BLA BLU :(
        
        this.setState(() => { return { waitForLeave: false, joined: false }; })
        
        console.log(result);
    }

    private getChatChannel(): string {
        if (this.state.game == null) {
            return null;
        }

        return this.state.game.phase == "Day"
            ? this.getDayChatChannel() : this.getNightChatChannel();
    }
    
    private getDayChatChannel(): string {        
        return !this.state.lynched ? "everyone" : "";
    }

    private getNightChatChannel(): string {
        const isMafia = this.service.isMafia(this.state.role);
        return (this.state.lynched || this.service.isMedium(this.state.role)) 
            ? "graveyard" : isMafia 
            ? "mafia" : "";;
    }

    private updateGameState(): void {
        this.service.getStateAsync().then(result => {
            try {
                
                let hasJoined = false;
                let lynched = false;
                let state = GameStateType.INVALID;
                let game: GameInfo = null;
                if (result != null) {
                    hasJoined = result.hasJoined;
                    lynched = result.lynched;
                    state = result.state;                 
                    game = result.game;
                } 

                this.setState(() => {
                    return { joined: hasJoined, state: state, game: game, lynched };
                })
            } finally {
                this.rescheduleGetState();
            }

            // the result of this should be whether game is: joinable | in progress | not started
            // also: am I in game? true | false

            // if joinable, show join button (anonymous | keep-track), keep-track can refresh website without leaving game. 
            //      anonymous will open slot for 1 day+1 night when leaving game. then character has commited suicide. Character can also be killed
            //      during that period of time. It is possible for anon to rejoin.
            // if in progress or not started, queue for next game (only shared id)
            // if in progress, leaving game is possible. But if player leaves on their own. Character commits suicide.
        });
    }

    addChatMessage(msg: ChatMessage): void {
        const existing = this.state.chatMessages.find(x => 
            x.timeSent == msg.timeSent && 
            x.sender == msg.sender &&
            x.channel == msg.channel &&
            x.message == msg.message);

        if (existing) {
            console.warn("Trying to add a duplicated chat message");
            return;
        }

        this.state.chatMessages.push(msg);
        this.setState({
            chatMessages: this.state.chatMessages
        });
    }

    private getChatMessages(): void {
        this.service.getChatMessagesAsync(this.getChatChannel()).then(result => {
            this.rescheduleGetChatMessages();
        })
    }
    
    private rescheduleGetChatMessages(): void {
        if (this.chatTimer) window.clearTimeout(this.chatTimer);
        this.chatTimer = window.setTimeout(() => {
            this.getChatMessages();
        }, 50);
    }

    
    private rescheduleGetState(): void {
        if (this.stateTimer) window.clearTimeout(this.stateTimer);
        this.stateTimer = window.setTimeout(() => {
            this.updateGameState();            
        }, 50);
    }
}