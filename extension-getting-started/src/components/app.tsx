import * as React from "react";

import TwitchComponentState from "../twitch-component-state";
import BlakullaService from "../blakulla-service";

export interface AppProps { compiler: string; framework: string; }

export class App extends React.Component<AppProps, TwitchComponentState> {
    private readonly service: BlakullaService;
    constructor(props: AppProps) {
        super(props);
        this.service = new BlakullaService();
        this.state = new TwitchComponentState();
    }
    render() {
        if (this.service.isReady) {
            return (
                <div className="App">
                    <div className={this.state.theme === 'light' ? 'App-light' : 'App-dark'} >
                        <p>{JSON.stringify(this.state.test)}</p>
                        <p>Hello world!</p>
                        <p>My token is: {this.service.auth.state.token}</p>
                        <p>My opaque ID is {this.service.auth.getOpaqueId()}.</p>
                        <div>{this.service.auth.isModerator() ? <p>I am currently a mod, and here's a special mod button <input value='mod button' type='button' /></p> : 'I am currently not a mod.'}</div>
                        <p>I have {this.service.auth.hasSharedId() ? `shared my ID, and my user_id is ${this.service.auth.getUserId()}` : 'not shared my ID'}.</p>
                    </div>
                </div>
            )
        } else {
            return (
                <div className="App">
                    Loading...
                </div>
            )
        }
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

            this.service.getGameStateAsync().then(result => {
                if (result == null) {
                    // not necessarily an error and don't want to throw an exception when awaited, so we return null instead
                    return;
                }

                // the result of this should be whether game is: joinable | in progress | not started
                // also: am I in game? true | false

                // if joinable, show join button (anonymous | keep-track), keep-track can refresh website without leaving game. 
                //      anonymous will open slot for 1 day+1 night when leaving game. then character has commited suicide. Character can also be killed
                //      during that period of time. It is possible for anon to rejoin.
                // if in progress or not started, queue for next game (only shared id)
                // if in progress, leaving game is possible. But if player leaves on their own. Character commits suicide.

            });

            // this.service.leaveGameAsync()
            // this.service.joinGameAsync()            
            // this.service.voteTrialAsync(player)
            // this.service.voteJudgementAsync(guilty|innocent)
            // this.service.updateLastWillAsync(text)
            // this.service.updateDeathNoteAsync(text)
            // this.service.useAbilityAsync(id,arg0,arg1,argN...)
            // this.service.canUseAbilityAsync()

            this.service.testAsync().then(result => {
                this.setState(() => {
                    return { test: result }
                })
            });

        },
            (visibility: boolean) => this.visibilityChanged(visibility),
            (context: any, delta: any) => this.contextUpdate(context, delta));
    }

    componentWillUnmount() {
        this.service.dispose();
    }
}