import React from 'react';
import Heading from '../../../components/Heading';
import AuctionForm from '../../AuctionForm';
import { getDetailView } from '../../../actions/auctionActions';

export default async function Update({ params }: { params: { id: string } }) {
	const data = await getDetailView(params.id);
	return (
		<div className='mx-auto max-w-[75%] shadow-lg p-10 bg-white rounded-lg'>
			<Heading
				title='Update your auction'
				subtitle='Please update the details of your car'
			/>
			<AuctionForm auction={data} />
		</div>
	);
}
