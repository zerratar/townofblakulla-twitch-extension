import * as React from "react";

import GameState from "../game-state";
import BlakullaService from "../blakulla-service";
import { ToggleOverlayButton } from "./toggle-overlay-button";

import "./app.scss";

export interface AppProps { }

export enum GameStateType {
    JOINABLE = 0,
    STARTED = 1,
    NOT_STARTED = 2
}

export class App extends React.Component<AppProps, GameState> {
    private readonly service: BlakullaService;
    constructor(props: AppProps) {
        super(props);
        this.service = new BlakullaService();
        this.state = new GameState();
        this.leaveGameAsync = this.leaveGameAsync.bind(this);
        this.joinGameAsync = this.joinGameAsync.bind(this);
        this.onNameChanged = this.onNameChanged.bind(this);
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

        if (!this.service.isReady || this.state.state == -1 || this.state.state == GameStateType.NOT_STARTED) {
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
        } else {
            return (
                <div className="App">
                    <ToggleOverlayButton onVisibilityChanged={this.visibilityChanged} />
                </div>
            )
        }
    }

    renderGame() {
        return (
            <div className="App">
                <ToggleOverlayButton onVisibilityChanged={this.visibilityChanged} />
                <button onClick={this.leaveGameAsync}>Leave</button>
            </div>
        )
    }

    renderJoinGame() {
        return (
            <div className="App">
                <ToggleOverlayButton onVisibilityChanged={this.visibilityChanged} />
                <div className="join-panel">
                    <div className="game-logo">
                        <img src="./images/logo.png"></img>
                    </div>

                    <div className="join-panel-input">                        
                        <img className="bg-image" src="./images/frame-tiny.png"></img>
                        <input placeholder="Enter a name" value={this.state.name} onChange={this.onNameChanged} />
                        <button onClick={this.joinGameAsync}>Join</button>
                    </div>
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
        this.service.start((auth: any) => {

            this.service.getStateAsync().then(result => {
                if (result == null) {
                    // not necessarily an error and don't want to throw an exception when awaited, so we return null instead
                    return;
                }

                // result: {  "hasJoined": false|true, "state": 0|1|2  }

                this.setState(() => {
                    return { joined: result.hasJoined, state: result.state };
                })
                console.log(JSON.stringify(result));

                // the result of this should be whether game is: joinable | in progress | not started
                // also: am I in game? true | false

                // if joinable, show join button (anonymous | keep-track), keep-track can refresh website without leaving game. 
                //      anonymous will open slot for 1 day+1 night when leaving game. then character has commited suicide. Character can also be killed
                //      during that period of time. It is possible for anon to rejoin.
                // if in progress or not started, queue for next game (only shared id)
                // if in progress, leaving game is possible. But if player leaves on their own. Character commits suicide.

            });

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
        this.service.dispose();
    }

    onNameChanged(e: React.ChangeEvent<HTMLInputElement>) {
        const name = e.currentTarget.value;
        this.setState(() => {
            return { name };
        });
    }

    async joinGameAsync() {
        if (!this.state.name || this.state.name == null || this.state.name.trim().length == 0) {
            return;
        }

        console.log(`${this.state.name}, want to join the game, huh?`);
        await this.service.joinAsync(this.state.name);
    }

    async leaveGameAsync() {
        console.log("NOOOOO!!! Please don't leave :(");
        await this.service.leaveAsync(); // BLA BLU :(
    }
}