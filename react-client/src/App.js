import './App.css';
import { CreateListingView } from './Components/Views/CreateListingView';
import { CreateListingButton } from './Components/Buttons/CreateListingButton';
import { ListOfListings } from './Components/ListOfListings';
import { Component } from 'react';
import axios from 'axios';

class App extends Component {
    createListingButtonId = "create-listing-button";
    createListingButton;

    constructor(props) {
        super(props)
        this.state = {
            listings: true,
            createListingView: false,
        }

        this.createListingButton = document.createElement('button');

        this.toggleCreateListing = this.toggleCreateListing.bind(this);
    }

    toggleCreateListing = (toggleBool) => {
        this.setState({ createListingView: toggleBool });
    }

    componentDidMount() {
        this.createListingButton = document.getElementById(this.createListingButtonId);

        this.createListingButton.addEventListener('click', () => {
            this.toggleCreateListing(true);
        })

        /* axios example
        this.buttonElement.addEventListener('click', () => {
            axios({
                method: 'get',
                url: 'https://localhost:7030/servertest/',
                data: {}
            }).then((response) => {
                this.setState({ response: JSON.stringify(response.data) });
            })
        })
        */
    }

    render() {
        return (
            <>
                {
                    !this.state.createListingView
                    ?
                        <div className="listings-container">
                            <>
                                <CreateListingButton
                                    id="create-listing-button"
                                    text="New listing"
                                    class="btn"
                                />
                                { this.state.listings && <ListOfListings /> }
                            </>
                        </div>
                    :
                        <CreateListingView />
                }
                
            </>
        );
    } 
}

export default App;
